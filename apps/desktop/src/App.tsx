import {
  AlertTriangle,
  Archive,
  BookOpen,
  Check,
  CircleAlert,
  CircleCheck,
  ClipboardList,
  FileText,
  HeartPulse,
  Library,
  ListChecks,
  LoaderCircle,
  Plus,
  RefreshCw,
  Save,
  Settings,
  Shield,
  Sparkle,
  Trash2,
  UserPlus
} from "lucide-react";
import { useEffect, useMemo, useState, type ReactNode } from "react";
import { ApiError, ValidationApiError } from "./api/client";
import {
  createDraft,
  deleteCharacter,
  finalizeCharacter,
  getCharacter,
  listCharacters,
  updateCharacter,
  validateCharacter
} from "./api/characters";
import type {
  AssetDefinitionDto,
  BondDto,
  CharacterDto,
  CharacterSummaryDto,
  DebilityDefinitionDto,
  HealthDto,
  ProgressTrackDto,
  RulesetDto,
  SelectedAssetDto,
  ValidationIssueDto,
  ValidationResultDto,
  VowDto
} from "./api/contracts";
import { getHealth, getRuleset } from "./api/rulesets";
import {
  Badge,
  Button,
  Card,
  Checkbox,
  Field,
  FieldError,
  Input,
  Meter,
  Notice,
  Panel,
  Select,
  SidebarItem,
  Tabs,
  Textarea
} from "./design-system";

type AppView = "dashboard" | "characters" | "create" | "character" | "settings";
type CharacterSubView = "overview" | "assets" | "bonds" | "vows" | "conditions";
type WizardStepId = "concept" | "stats" | "assets" | "bonds" | "vows" | "review";

const wizardSteps: Array<{ id: WizardStepId; label: string }> = [
  { id: "concept", label: "Concept" },
  { id: "stats", label: "Stats" },
  { id: "assets", label: "Assets" },
  { id: "bonds", label: "Bonds" },
  { id: "vows", label: "Starting Vow" },
  { id: "review", label: "Review" }
];

const rankFallback = ["troublesome", "dangerous", "formidable", "extreme", "epic"];

export function App() {
  const [activeCharacter, setActiveCharacter] = useState<CharacterDto | null>(null);
  const [activeSubView, setActiveSubView] = useState<CharacterSubView>("overview");
  const [characters, setCharacters] = useState<CharacterSummaryDto[]>([]);
  const [draft, setDraft] = useState<CharacterDto | null>(null);
  const [health, setHealth] = useState<HealthDto | null>(null);
  const [isBusy, setIsBusy] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [lastSavedAt, setLastSavedAt] = useState<string | null>(null);
  const [ruleset, setRuleset] = useState<RulesetDto | null>(null);
  const [step, setStep] = useState<WizardStepId>("concept");
  const [systemError, setSystemError] = useState<string | null>(null);
  const [validation, setValidation] = useState<ValidationResultDto | null>(null);
  const [view, setView] = useState<AppView>("characters");

  useEffect(() => {
    void loadAppData();
  }, []);

  const issues = useMemo(() => validationIssues(validation), [validation]);
  const issueByField = useMemo(() => groupIssuesByField(issues), [issues]);
  const issueByStep = useMemo(() => groupIssuesByStep(issues), [issues]);

  async function loadAppData() {
    setIsLoading(true);
    setSystemError(null);

    try {
      const [healthResult, rulesetResult, characterResult] = await Promise.all([
        getHealth(),
        getRuleset("ironsworn"),
        listCharacters()
      ]);
      setHealth(healthResult);
      setRuleset(rulesetResult);
      setCharacters(characterResult.items);
    } catch (error) {
      setSystemError(toErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
  }

  async function refreshCharacters() {
    const result = await listCharacters();
    setCharacters(result.items);
  }

  async function startNewCharacter() {
    setIsBusy(true);
    setSystemError(null);
    setValidation(null);

    try {
      const createdDraft = await createDraft({ rulesetId: "ironsworn" });
      setDraft(createdDraft);
      setStep("concept");
      setView("create");
      setLastSavedAt(createdDraft.updatedAt);
      await refreshCharacters();
    } catch (error) {
      setSystemError(toErrorMessage(error));
    } finally {
      setIsBusy(false);
    }
  }

  async function openCharacter(characterId: string, mode: "view" | "resume" = "view") {
    setIsBusy(true);
    setSystemError(null);
    setValidation(null);

    try {
      const character = await getCharacter(characterId);
      if (mode === "resume" || character.status === "draft") {
        setDraft(character);
        setStep("concept");
        setView("create");
      } else {
        setActiveCharacter(character);
        setActiveSubView("overview");
        setView("character");
      }
    } catch (error) {
      setSystemError(toErrorMessage(error));
    } finally {
      setIsBusy(false);
    }
  }

  async function saveDraft(nextDraft = draft) {
    if (!nextDraft) {
      return null;
    }

    setIsBusy(true);
    setSystemError(null);

    try {
      const saved = await updateCharacter(nextDraft);
      setDraft(saved);
      setLastSavedAt(saved.updatedAt);
      await refreshCharacters();
      return saved;
    } catch (error) {
      if (error instanceof ValidationApiError) {
        setValidation(error.validation);
      } else {
        setSystemError(toErrorMessage(error));
      }
      return null;
    } finally {
      setIsBusy(false);
    }
  }

  async function runValidation() {
    if (!draft) {
      return;
    }

    const saved = await saveDraft();
    if (!saved) {
      return;
    }

    try {
      setValidation(await validateCharacter(saved.id));
    } catch (error) {
      setSystemError(toErrorMessage(error));
    }
  }

  async function finalizeDraft() {
    if (!draft) {
      return;
    }

    const saved = await saveDraft();
    if (!saved) {
      return;
    }

    setIsBusy(true);
    setSystemError(null);

    try {
      const finalized = await finalizeCharacter(saved.id);
      setActiveCharacter(finalized);
      setDraft(null);
      setValidation(null);
      setView("character");
      setActiveSubView("overview");
      await refreshCharacters();
    } catch (error) {
      if (error instanceof ValidationApiError) {
        setValidation(error.validation);
        setStep("review");
      } else {
        setSystemError(toErrorMessage(error));
      }
    } finally {
      setIsBusy(false);
    }
  }

  async function deleteCharacterRecord(characterId: string) {
    setIsBusy(true);
    setSystemError(null);

    try {
      await deleteCharacter(characterId);
      if (activeCharacter?.id === characterId) {
        setActiveCharacter(null);
        setView("characters");
      }
      await refreshCharacters();
    } catch (error) {
      setSystemError(toErrorMessage(error));
    } finally {
      setIsBusy(false);
    }
  }

  function patchDraft(patcher: (character: CharacterDto) => CharacterDto) {
    setDraft((current) => (current ? patcher(current) : current));
  }

  function navigate(viewName: AppView) {
    setView(viewName);
    if (viewName !== "character") {
      setActiveSubView("overview");
    }
  }

  return (
    <main className="app-shell">
      <aside className="app-sidebar" aria-label="Primary navigation">
        <div className="app-brand">
          <Shield aria-hidden="true" />
          <div>
            <strong>CharacterManager</strong>
            <span>Ironsworn MVP</span>
          </div>
        </div>

        <nav className="app-nav">
          <SidebarItem active={view === "dashboard"} icon={<ClipboardList />} onClick={() => navigate("dashboard")}>
            Dashboard
          </SidebarItem>
          <SidebarItem active={view === "characters"} icon={<Library />} onClick={() => navigate("characters")}>
            Characters
          </SidebarItem>
          <SidebarItem active={view === "create"} icon={<UserPlus />} onClick={() => (draft ? navigate("create") : void startNewCharacter())}>
            Create Character
          </SidebarItem>
          <SidebarItem active={view === "settings"} icon={<Settings />} onClick={() => navigate("settings")}>
            Settings
          </SidebarItem>
        </nav>

        {activeCharacter ? (
          <section className="active-character-summary" aria-label="Active character">
            <span>Active Character</span>
            <strong>{displayCharacterName(activeCharacter)}</strong>
            <Badge tone={activeCharacter.status === "finalized" ? "success" : "warning"}>
              {activeCharacter.status}
            </Badge>
          </section>
        ) : null}
      </aside>

      <section className="app-workspace">
        {systemError ? (
          <Notice tone="danger" icon={<CircleAlert />}>
            <strong>Backend unavailable or request failed.</strong>
            <p>{systemError}</p>
            <Button variant="secondary" icon={<RefreshCw />} onClick={() => void loadAppData()}>
              Retry
            </Button>
          </Notice>
        ) : null}

        {isLoading ? (
          <LoadingView />
        ) : (
          <>
            {view === "dashboard" ? (
              <DashboardView
                characters={characters}
                health={health}
                onNewCharacter={startNewCharacter}
                onOpenCharacter={openCharacter}
              />
            ) : null}
            {view === "characters" ? (
              <CharacterLibraryView
                characters={characters}
                isBusy={isBusy}
                onDelete={deleteCharacterRecord}
                onNewCharacter={startNewCharacter}
                onOpenCharacter={openCharacter}
              />
            ) : null}
            {view === "create" ? (
              <CreateCharacterWizard
                draft={draft}
                issueByField={issueByField}
                issueByStep={issueByStep}
                isBusy={isBusy}
                lastSavedAt={lastSavedAt}
                onFinalize={finalizeDraft}
                onNewDraft={startNewCharacter}
                onPatchDraft={patchDraft}
                onSaveDraft={saveDraft}
                onStepChange={setStep}
                onValidate={runValidation}
                ruleset={ruleset}
                step={step}
                validation={validation}
              />
            ) : null}
            {view === "character" && activeCharacter ? (
              <CharacterLayoutView
                character={activeCharacter}
                onBackToLibrary={() => navigate("characters")}
                onSubViewChange={setActiveSubView}
                ruleset={ruleset}
                subView={activeSubView}
              />
            ) : null}
            {view === "settings" ? (
              <SettingsView
                health={health}
                onRefresh={loadAppData}
                ruleset={ruleset}
              />
            ) : null}
          </>
        )}
      </section>
    </main>
  );
}

function LoadingView() {
  return (
    <section className="centered-state" aria-label="Loading app data">
      <LoaderCircle aria-hidden="true" className="spin-icon" />
      <h1>Opening the local archive</h1>
      <p>Loading rules metadata and saved characters.</p>
    </section>
  );
}

function DashboardView({
  characters,
  health,
  onNewCharacter,
  onOpenCharacter
}: {
  characters: CharacterSummaryDto[];
  health: HealthDto | null;
  onNewCharacter: () => void;
  onOpenCharacter: (characterId: string, mode?: "view" | "resume") => void;
}) {
  const drafts = characters.filter((character) => character.status === "draft");
  const finalized = characters.filter((character) => character.status === "finalized");
  const recent = finalized[0] ?? drafts[0] ?? null;

  return (
    <section className="screen-stack">
      <PageHeader
        eyebrow="Dashboard"
        title="Character operations"
        actions={
          <Button icon={<Plus />} onClick={onNewCharacter}>
            New Character
          </Button>
        }
      >
        <p>Resume a draft, open a character, or start a standard Ironsworn character.</p>
      </PageHeader>

      <div className="summary-grid">
        <Panel title="Local service" eyebrow="Status">
          <DefinitionList
            items={[
              ["Service", health?.service ?? "Unavailable"],
              ["Version", health?.version ?? "Unknown"]
            ]}
          />
        </Panel>
        <Panel title="Library" eyebrow="Characters">
          <DefinitionList
            items={[
              ["Finalized", finalized.length.toString()],
              ["Drafts", drafts.length.toString()],
              ["Needs review", characters.filter((character) => character.needsReview).length.toString()]
            ]}
          />
        </Panel>
      </div>

      {recent ? (
        <Panel title="Resume" eyebrow="Recent">
          <CharacterSummaryCard
            character={recent}
            onDelete={() => undefined}
            onOpen={() => onOpenCharacter(recent.id, recent.status === "draft" ? "resume" : "view")}
            showDelete={false}
          />
        </Panel>
      ) : (
        <EmptyLibraryState onNewCharacter={onNewCharacter} />
      )}
    </section>
  );
}

function CharacterLibraryView({
  characters,
  isBusy,
  onDelete,
  onNewCharacter,
  onOpenCharacter
}: {
  characters: CharacterSummaryDto[];
  isBusy: boolean;
  onDelete: (characterId: string) => void;
  onNewCharacter: () => void;
  onOpenCharacter: (characterId: string, mode?: "view" | "resume") => void;
}) {
  return (
    <section className="screen-stack">
      <PageHeader
        eyebrow="Characters"
        title="Character Library"
        actions={
          <Button disabled={isBusy} icon={<Plus />} onClick={onNewCharacter}>
            New Character
          </Button>
        }
      >
        <p>Saved characters and drafts from the local backend.</p>
      </PageHeader>

      {characters.length === 0 ? (
        <EmptyLibraryState onNewCharacter={onNewCharacter} />
      ) : (
        <section className="card-grid" aria-label="Saved characters">
          {characters.map((character) => (
            <CharacterSummaryCard
              character={character}
              key={character.id}
              onDelete={() => onDelete(character.id)}
              onOpen={() => onOpenCharacter(character.id, character.status === "draft" ? "resume" : "view")}
            />
          ))}
        </section>
      )}
    </section>
  );
}

function EmptyLibraryState({ onNewCharacter }: { onNewCharacter: () => void }) {
  return (
    <Panel className="empty-panel">
      <Archive aria-hidden="true" />
      <h2>No local characters yet</h2>
      <p>Create a standard Ironsworn character draft and finalize it when validation passes.</p>
      <Button icon={<Plus />} onClick={onNewCharacter}>
        New Character
      </Button>
    </Panel>
  );
}

function CharacterSummaryCard({
  character,
  onDelete,
  onOpen,
  showDelete = true
}: {
  character: CharacterSummaryDto;
  onDelete: () => void;
  onOpen: () => void;
  showDelete?: boolean;
}) {
  const statusTone = character.status === "finalized" && !character.needsReview ? "success" : character.needsReview ? "warning" : "info";

  return (
    <Card elevated={character.needsReview}>
      <div className="card-title-row">
        <div>
          <h3>{character.displayName}</h3>
          <p>{character.concept || "No concept recorded."}</p>
        </div>
        <Badge tone={statusTone}>{character.needsReview ? "Needs review" : character.status}</Badge>
      </div>
      <DefinitionList
        compact
        items={[
          ["Health", character.stateSummary.health.toString()],
          ["Spirit", character.stateSummary.spirit.toString()],
          ["Supply", character.stateSummary.supply.toString()],
          ["Momentum", character.stateSummary.momentum.toString()],
          ["Active vows", character.stateSummary.activeVowCount.toString()]
        ]}
      />
      {character.error ? (
        <Notice tone="warning" icon={<AlertTriangle />}>
          {character.error.message}
        </Notice>
      ) : null}
      <div className="button-row">
        <Button variant="secondary" icon={<BookOpen />} onClick={onOpen}>
          {character.status === "draft" ? "Resume Draft" : "Open Character"}
        </Button>
        {showDelete ? (
          <Button variant="danger" icon={<Trash2 />} onClick={onDelete}>
            Delete
          </Button>
        ) : null}
      </div>
    </Card>
  );
}

function CreateCharacterWizard({
  draft,
  issueByField,
  issueByStep,
  isBusy,
  lastSavedAt,
  onFinalize,
  onNewDraft,
  onPatchDraft,
  onSaveDraft,
  onStepChange,
  onValidate,
  ruleset,
  step,
  validation
}: {
  draft: CharacterDto | null;
  issueByField: Map<string, ValidationIssueDto[]>;
  issueByStep: Map<string, ValidationIssueDto[]>;
  isBusy: boolean;
  lastSavedAt: string | null;
  onFinalize: () => void;
  onNewDraft: () => void;
  onPatchDraft: (patcher: (character: CharacterDto) => CharacterDto) => void;
  onSaveDraft: () => void;
  onStepChange: (step: WizardStepId) => void;
  onValidate: () => void;
  ruleset: RulesetDto | null;
  step: WizardStepId;
  validation: ValidationResultDto | null;
}) {
  if (!draft) {
    return (
      <section className="screen-stack">
        <PageHeader eyebrow="Character Creation" title="Create Character">
          <p>No active draft is loaded.</p>
        </PageHeader>
        <Button icon={<Plus />} onClick={onNewDraft}>
          Create Draft
        </Button>
      </section>
    );
  }

  const currentStepIndex = wizardSteps.findIndex((candidate) => candidate.id === step);
  const canGoBack = currentStepIndex > 0;
  const canContinue = currentStepIndex < wizardSteps.length - 1;

  function continueStep() {
    if (canContinue) {
      onStepChange(wizardSteps[currentStepIndex + 1].id);
    }
  }

  function backStep() {
    if (canGoBack) {
      onStepChange(wizardSteps[currentStepIndex - 1].id);
    }
  }

  return (
    <section className="screen-stack">
      <PageHeader
        eyebrow="Character Creation"
        title="Create Character"
        actions={<Badge tone={draft.status === "draft" ? "info" : "success"}>{draft.status}</Badge>}
      >
        <p>Drafts save to the local backend. Final creation uses backend validation.</p>
      </PageHeader>

      <div className="wizard-layout">
        <nav className="wizard-steps" aria-label="Creation steps">
          {wizardSteps.map((wizardStep, index) => {
            const stepIssues = issueByStep.get(wizardStep.id) ?? [];
            const isActive = wizardStep.id === step;
            const isPast = index < currentStepIndex;
            return (
              <button
                aria-current={isActive ? "step" : undefined}
                className={`wizard-step ${isActive ? "active" : ""} ${isPast ? "complete" : ""}`}
                key={wizardStep.id}
                onClick={() => onStepChange(wizardStep.id)}
                type="button"
              >
                <span>{index + 1}</span>
                <strong>{wizardStep.label}</strong>
                {stepIssues.length ? <Badge tone="danger">{stepIssues.length}</Badge> : null}
              </button>
            );
          })}
        </nav>

        <Panel className="wizard-panel">
          {step === "concept" ? (
            <ConceptStep draft={draft} issueByField={issueByField} onPatchDraft={onPatchDraft} />
          ) : null}
          {step === "stats" ? (
            <StatsStep draft={draft} issueByField={issueByField} onPatchDraft={onPatchDraft} ruleset={ruleset} />
          ) : null}
          {step === "assets" ? (
            <AssetsStep draft={draft} issueByField={issueByField} onPatchDraft={onPatchDraft} ruleset={ruleset} />
          ) : null}
          {step === "bonds" ? (
            <BondsStep draft={draft} issueByField={issueByField} onPatchDraft={onPatchDraft} ruleset={ruleset} />
          ) : null}
          {step === "vows" ? (
            <VowsStep draft={draft} issueByField={issueByField} onPatchDraft={onPatchDraft} ruleset={ruleset} />
          ) : null}
          {step === "review" ? (
            <ReviewStep
              draft={draft}
              onStepChange={onStepChange}
              ruleset={ruleset}
              validation={validation}
            />
          ) : null}

          <footer className="wizard-footer">
            <div className="save-status">
              {lastSavedAt ? <span>Saved {formatDate(lastSavedAt)}</span> : <span>Draft not saved yet</span>}
            </div>
            <div className="button-row">
              <Button disabled={!canGoBack || isBusy} variant="secondary" onClick={backStep}>
                Back
              </Button>
              <Button disabled={isBusy} icon={<Save />} variant="secondary" onClick={() => void onSaveDraft()}>
                Save Draft
              </Button>
              {step === "review" ? (
                <>
                  <Button disabled={isBusy} icon={<ListChecks />} variant="secondary" onClick={() => void onValidate()}>
                    Validate
                  </Button>
                  <Button disabled={isBusy} icon={<Check />} onClick={() => void onFinalize()}>
                    Finalize
                  </Button>
                </>
              ) : (
                <Button disabled={isBusy} onClick={continueStep}>
                  Continue
                </Button>
              )}
            </div>
          </footer>
        </Panel>
      </div>
    </section>
  );
}

function ConceptStep({
  draft,
  issueByField,
  onPatchDraft
}: {
  draft: CharacterDto;
  issueByField: Map<string, ValidationIssueDto[]>;
  onPatchDraft: (patcher: (character: CharacterDto) => CharacterDto) => void;
}) {
  return (
    <section className="form-stack">
      <StepHeading title="Character Concept" text="Capture the identity first. Equipment stays narrative in this MVP." />
      <Field label="Name">
        <Input
          value={draft.name}
          onChange={(event) => onPatchDraft((character) => ({ ...character, name: event.target.value }))}
        />
        <FieldError>{firstIssue(issueByField, "name")}</FieldError>
      </Field>
      <Field label="Concept">
        <Input
          value={draft.concept}
          onChange={(event) => onPatchDraft((character) => ({ ...character, concept: event.target.value }))}
        />
      </Field>
      <Field label="Background notes">
        <Textarea
          value={draft.backgroundNotes}
          onChange={(event) => onPatchDraft((character) => ({ ...character, backgroundNotes: event.target.value }))}
        />
      </Field>
      <Field label="Equipment notes">
        <Textarea
          value={draft.equipmentNotes}
          onChange={(event) => onPatchDraft((character) => ({ ...character, equipmentNotes: event.target.value }))}
        />
      </Field>
    </section>
  );
}

function StatsStep({
  draft,
  issueByField,
  onPatchDraft,
  ruleset
}: {
  draft: CharacterDto;
  issueByField: Map<string, ValidationIssueDto[]>;
  onPatchDraft: (patcher: (character: CharacterDto) => CharacterDto) => void;
  ruleset: RulesetDto | null;
}) {
  const stats = ruleset?.stats ?? [];
  const values = ruleset?.startingCharacter.statArray ?? [3, 2, 2, 1, 1];
  const assignedValues = stats.map((stat) => draft.stats[stat.id] ?? 0).filter(Boolean);

  return (
    <section className="form-stack">
      <StepHeading title="Stats" text="Assign the standard starting array: one 3, two 2s, and two 1s." />
      <div className="value-chip-row" aria-label="Starting stat values">
        {values.map((value, index) => (
          <span className={assignedValues.includes(value) ? "value-chip used" : "value-chip"} key={`${value}-${index}`}>
            {value}
          </span>
        ))}
      </div>
      <FieldError>{firstIssue(issueByField, "stats")}</FieldError>
      <div className="stat-grid">
        {stats.map((stat) => (
          <Field helpText={stat.summary} key={stat.id} label={stat.name}>
            <Select
              value={draft.stats[stat.id] || 0}
              onChange={(event) =>
                onPatchDraft((character) => ({
                  ...character,
                  stats: { ...character.stats, [stat.id]: Number(event.target.value) }
                }))
              }
            >
              <option value={0}>Unassigned</option>
              <option value={3}>3</option>
              <option value={2}>2</option>
              <option value={1}>1</option>
            </Select>
            <FieldError>{firstIssue(issueByField, `stats.${stat.id}`)}</FieldError>
          </Field>
        ))}
      </div>
    </section>
  );
}

function AssetsStep({
  draft,
  issueByField,
  onPatchDraft,
  ruleset
}: {
  draft: CharacterDto;
  issueByField: Map<string, ValidationIssueDto[]>;
  onPatchDraft: (patcher: (character: CharacterDto) => CharacterDto) => void;
  ruleset: RulesetDto | null;
}) {
  const assets = ruleset?.assets ?? [];
  const requiredCount = ruleset?.startingCharacter.requiredAssetCount ?? 3;

  function toggleAsset(asset: AssetDefinitionDto, checked: boolean) {
    onPatchDraft((character) => {
      const nextAssets = checked
        ? [
            ...character.assets,
            {
              definitionId: asset.id,
              selectedAbilityIds: asset.abilities[0]?.id ? [asset.abilities[0].id] : [],
              companionName: null,
              companionHealth: asset.requiresCompanionName ? 5 : null,
              notes: "",
              sourceReference: asset.sourceReference
            }
          ]
        : character.assets.filter((selectedAsset) => selectedAsset.definitionId !== asset.id);

      return { ...character, assets: nextAssets };
    });
  }

  function patchAsset(assetId: string, patch: Partial<SelectedAssetDto>) {
    onPatchDraft((character) => ({
      ...character,
      assets: character.assets.map((selectedAsset) =>
        selectedAsset.definitionId === assetId ? { ...selectedAsset, ...patch } : selectedAsset
      )
    }));
  }

  return (
    <section className="form-stack">
      <StepHeading title="Assets" text="Choose three metadata-only starting assets. Full asset card text is intentionally not displayed." />
      <div className="selected-count">
        <Badge tone={draft.assets.length === requiredCount ? "success" : "warning"}>
          {draft.assets.length} of {requiredCount} selected
        </Badge>
        <FieldError>{firstIssue(issueByField, "assets")}</FieldError>
      </div>
      {assets.length === 0 ? (
        <Notice tone="warning" icon={<AlertTriangle />}>
          Asset catalog is unavailable. Draft save is allowed, but final creation requires catalog validation.
        </Notice>
      ) : (
        <div className="asset-grid">
          {assets.map((asset) => {
            const selected = draft.assets.find((selectedAsset) => selectedAsset.definitionId === asset.id);
            return (
              <Card elevated={Boolean(selected)} key={asset.id}>
                <div className="card-title-row">
                  <div>
                    <span className="meta-label">{asset.type}</span>
                    <h3>{asset.name}</h3>
                  </div>
                  <Checkbox
                    checked={Boolean(selected)}
                    label="Select"
                    onChange={(event) => toggleAsset(asset, event.target.checked)}
                  />
                </div>
                <p>{asset.summary}</p>
                {selected && asset.requiresCompanionName ? (
                  <Field label="Companion name">
                    <Input
                      value={selected.companionName ?? ""}
                      onChange={(event) => patchAsset(asset.id, { companionName: event.target.value })}
                    />
                    <FieldError>{firstIssue(issueByField, `assets.${asset.id}.companionName`)}</FieldError>
                  </Field>
                ) : null}
              </Card>
            );
          })}
        </div>
      )}
    </section>
  );
}

function BondsStep({
  draft,
  issueByField,
  onPatchDraft,
  ruleset
}: {
  draft: CharacterDto;
  issueByField: Map<string, ValidationIssueDto[]>;
  onPatchDraft: (patcher: (character: CharacterDto) => CharacterDto) => void;
  ruleset: RulesetDto | null;
}) {
  const maxBonds = ruleset?.startingCharacter.maxStartingBonds ?? 3;

  function patchBond(id: string, patch: Partial<BondDto>) {
    onPatchDraft((character) => ({
      ...character,
      bonds: character.bonds.map((bond) => (bond.id === id ? { ...bond, ...patch } : bond))
    }));
  }

  return (
    <section className="form-stack">
      <StepHeading title="Bonds" text="Add up to three background bonds, or continue with none." />
      <div className="selected-count">
        <Badge tone={draft.bonds.length <= maxBonds ? "info" : "danger"}>
          {draft.bonds.length} of {maxBonds} used
        </Badge>
        <Button
          disabled={draft.bonds.length >= maxBonds}
          icon={<Plus />}
          variant="secondary"
          onClick={() => onPatchDraft((character) => ({ ...character, bonds: [...character.bonds, createBond()] }))}
        >
          Add Bond
        </Button>
      </div>
      <FieldError>{firstIssue(issueByField, "bonds")}</FieldError>
      {draft.bonds.length === 0 ? (
        <Notice tone="info" icon={<InfoIcon />}>
          Zero starting bonds is valid. You can add bonds later from the Bonds screen.
        </Notice>
      ) : null}
      <div className="form-stack">
        {draft.bonds.map((bond, index) => (
          <Card key={bond.id}>
            <div className="card-title-row">
              <h3>Bond {index + 1}</h3>
              <Button
                variant="danger"
                icon={<Trash2 />}
                onClick={() =>
                  onPatchDraft((character) => ({
                    ...character,
                    bonds: character.bonds.filter((candidate) => candidate.id !== bond.id)
                  }))
                }
              >
                Remove
              </Button>
            </div>
            <div className="two-column-form">
              <Field label="Name">
                <Input value={bond.name} onChange={(event) => patchBond(bond.id, { name: event.target.value })} />
                <FieldError>{firstIssue(issueByField, `bonds[${index}].name`)}</FieldError>
              </Field>
              <Field label="Type">
                <Input value={bond.type} onChange={(event) => patchBond(bond.id, { type: event.target.value })} />
              </Field>
              <Field label="Location">
                <Input value={bond.location} onChange={(event) => patchBond(bond.id, { location: event.target.value })} />
              </Field>
              <Field label="Notes">
                <Textarea value={bond.notes} onChange={(event) => patchBond(bond.id, { notes: event.target.value })} />
              </Field>
            </div>
          </Card>
        ))}
      </div>
    </section>
  );
}

function VowsStep({
  draft,
  issueByField,
  onPatchDraft,
  ruleset
}: {
  draft: CharacterDto;
  issueByField: Map<string, ValidationIssueDto[]>;
  onPatchDraft: (patcher: (character: CharacterDto) => CharacterDto) => void;
  ruleset: RulesetDto | null;
}) {
  const ranks = ruleset?.ranks ?? rankFallback;

  function ensureVow(kind: "background" | "inciting") {
    onPatchDraft((character) => {
      const existing = character.vows.find((vow) =>
        kind === "background" ? vow.isBackgroundVow : vow.isIncitingIncident
      );
      if (existing) {
        return character;
      }

      const created = createVow(kind);
      return {
        ...character,
        vows: [...character.vows, created.vow],
        progressTracks: [...character.progressTracks, created.progressTrack]
      };
    });
  }

  function patchVow(vowId: string, patch: Partial<VowDto>) {
    onPatchDraft((character) => ({
      ...character,
      vows: character.vows.map((vow) => {
        if (vow.id !== vowId) {
          return vow;
        }
        const nextVow = { ...vow, ...patch };
        return nextVow;
      }),
      progressTracks: character.progressTracks.map((track) => {
        const vow = character.vows.find((candidate) => candidate.id === vowId);
        if (!vow || track.id !== vow.progressTrackId) {
          return track;
        }
        return {
          ...track,
          name: patch.title ?? track.name,
          rank: patch.rank ?? track.rank
        };
      })
    }));
  }

  const backgroundVow = draft.vows.find((vow) => vow.isBackgroundVow);
  const incitingVow = draft.vows.find((vow) => vow.isIncitingIncident);

  return (
    <section className="form-stack">
      <StepHeading title="Starting Vow" text="At least one active vow is required. Both prompts are recommended for a ready-to-play start." />
      <FieldError>{firstIssue(issueByField, "vows")}</FieldError>
      <VowEditor
        issueByField={issueByField}
        label="Background vow"
        onCreate={() => ensureVow("background")}
        onPatch={patchVow}
        ranks={ranks}
        vow={backgroundVow}
        vowIndex={draft.vows.findIndex((vow) => vow.id === backgroundVow?.id)}
      />
      <VowEditor
        issueByField={issueByField}
        label="Inciting-incident vow"
        onCreate={() => ensureVow("inciting")}
        onPatch={patchVow}
        ranks={ranks}
        vow={incitingVow}
        vowIndex={draft.vows.findIndex((vow) => vow.id === incitingVow?.id)}
      />
    </section>
  );
}

function VowEditor({
  issueByField,
  label,
  onCreate,
  onPatch,
  ranks,
  vow,
  vowIndex
}: {
  issueByField: Map<string, ValidationIssueDto[]>;
  label: string;
  onCreate: () => void;
  onPatch: (vowId: string, patch: Partial<VowDto>) => void;
  ranks: string[];
  vow: VowDto | undefined;
  vowIndex: number;
}) {
  if (!vow) {
    return (
      <Card>
        <div className="card-title-row">
          <h3>{label}</h3>
          <Button icon={<Plus />} variant="secondary" onClick={onCreate}>
            Add
          </Button>
        </div>
      </Card>
    );
  }

  return (
    <Card>
      <h3>{label}</h3>
      <div className="two-column-form">
        <Field label="Title">
          <Input value={vow.title} onChange={(event) => onPatch(vow.id, { title: event.target.value })} />
          <FieldError>{firstIssue(issueByField, `vows[${vowIndex}].title`)}</FieldError>
        </Field>
        <Field label="Rank">
          <Select value={vow.rank} onChange={(event) => onPatch(vow.id, { rank: event.target.value })}>
            <option value="">Choose rank</option>
            {ranks.map((rank) => (
              <option key={rank} value={rank}>
                {formatLabel(rank)}
              </option>
            ))}
          </Select>
        </Field>
        <Field label="Notes">
          <Textarea value={vow.notes} onChange={(event) => onPatch(vow.id, { notes: event.target.value })} />
        </Field>
      </div>
    </Card>
  );
}

function ReviewStep({
  draft,
  onStepChange,
  ruleset,
  validation
}: {
  draft: CharacterDto;
  onStepChange: (step: WizardStepId) => void;
  ruleset: RulesetDto | null;
  validation: ValidationResultDto | null;
}) {
  const checklist = buildChecklist(draft, ruleset);
  const issues = validationIssues(validation);

  return (
    <section className="form-stack">
      <StepHeading title="Review" text="Review the character and finalize when backend validation accepts the draft." />
      <div className="review-grid">
        <Card title="Character">
          <DefinitionList
            items={[
              ["Name", displayCharacterName(draft)],
              ["Concept", draft.concept || "Not recorded"],
              ["Assets", draft.assets.length.toString()],
              ["Bonds", draft.bonds.length.toString()],
              ["Active vows", draft.vows.filter((vow) => vow.status === "active").length.toString()]
            ]}
          />
        </Card>
        <Card title="Checklist">
          <ul className="checklist">
            {checklist.map((item) => (
              <li key={item.label}>
                {item.complete ? <CircleCheck aria-hidden="true" /> : <CircleAlert aria-hidden="true" />}
                <span>{item.label}</span>
                <Button variant="secondary" onClick={() => onStepChange(item.step)}>
                  Open
                </Button>
              </li>
            ))}
          </ul>
        </Card>
      </div>
      <ValidationPanel issues={issues} onStepChange={onStepChange} />
      <Notice tone="warning" icon={<AlertTriangle />}>
        <p>
          TODO: The current validate endpoint checks draft-validity, while finalize returns standard creation errors.
          Use Finalize for the authoritative creation check until the backend exposes draft final-validation mode.
        </p>
      </Notice>
    </section>
  );
}

function CharacterLayoutView({
  character,
  onBackToLibrary,
  onSubViewChange,
  ruleset,
  subView
}: {
  character: CharacterDto;
  onBackToLibrary: () => void;
  onSubViewChange: (subView: CharacterSubView) => void;
  ruleset: RulesetDto | null;
  subView: CharacterSubView;
}) {
  return (
    <section className="screen-stack">
      <PageHeader
        eyebrow="Active Character"
        title={displayCharacterName(character)}
        actions={
          <Button variant="secondary" icon={<Library />} onClick={onBackToLibrary}>
            Library
          </Button>
        }
      >
        <p>{character.concept || "No concept recorded."}</p>
      </PageHeader>
      <Tabs
        onSelect={(label) => onSubViewChange(tabLabelToSubView(label))}
        tabs={[
          { label: "Overview", active: subView === "overview" },
          { label: "Assets", active: subView === "assets" },
          { label: "Bonds", active: subView === "bonds" },
          { label: "Vows", active: subView === "vows" },
          { label: "Conditions", active: subView === "conditions" }
        ]}
      />
      {subView === "overview" ? <CharacterOverviewView character={character} ruleset={ruleset} /> : null}
      {subView === "assets" ? <AssetsReadOnlyView character={character} ruleset={ruleset} /> : null}
      {subView === "bonds" ? <BondsReadOnlyView character={character} /> : null}
      {subView === "vows" ? <VowsReadOnlyView character={character} /> : null}
      {subView === "conditions" ? <ConditionsReadOnlyView character={character} ruleset={ruleset} /> : null}
    </section>
  );
}

function CharacterOverviewView({ character, ruleset }: { character: CharacterDto; ruleset: RulesetDto | null }) {
  return (
    <div className="overview-layout">
      <Panel title="Core State" eyebrow="Play">
        <div className="meter-grid">
          <MeterCard label="Health" meter={character.conditionMeters.health} />
          <MeterCard label="Spirit" meter={character.conditionMeters.spirit} />
          <MeterCard label="Supply" meter={character.conditionMeters.supply} />
          <Card>
            <span className="meta-label">Momentum</span>
            <strong className="big-value">{character.momentum.current}</strong>
            <DefinitionList
              compact
              items={[
                ["Max", character.momentum.max.toString()],
                ["Reset", character.momentum.reset.toString()],
                ["Minimum", character.momentum.minimum.toString()]
              ]}
            />
          </Card>
        </div>
      </Panel>
      <Panel title="Stats" eyebrow="Character">
        <div className="stat-pill-grid">
          {(ruleset?.stats ?? Object.keys(character.stats).map((id) => ({ id, name: formatLabel(id), summary: "", min: 1, max: 3, sourceReference: null }))).map((stat) => (
            <div className="stat-pill" key={stat.id}>
              <span>{stat.name}</span>
              <strong>{character.stats[stat.id] || 0}</strong>
            </div>
          ))}
        </div>
      </Panel>
      <Panel title="Vows & Progress" eyebrow="Summary">
        <ProgressList tracks={character.progressTracks} vows={character.vows} />
      </Panel>
      <Panel title="Assets, Bonds, Debilities" eyebrow="Summary">
        <DefinitionList
          items={[
            ["Assets", character.assets.length.toString()],
            ["Bonds", character.bonds.length.toString()],
            ["Marked debilities", character.debilities.filter((debility) => debility.marked).length.toString()],
            ["Experience", `${character.experience.available} available`]
          ]}
        />
      </Panel>
    </div>
  );
}

function AssetsReadOnlyView({ character, ruleset }: { character: CharacterDto; ruleset: RulesetDto | null }) {
  const catalog = new Map((ruleset?.assets ?? []).map((asset) => [asset.id, asset]));

  return (
    <section className="card-grid">
      {character.assets.length === 0 ? (
        <Notice tone="warning" icon={<AlertTriangle />}>No assets selected yet.</Notice>
      ) : (
        character.assets.map((asset) => {
          const definition = catalog.get(asset.definitionId);
          return (
            <Card key={asset.definitionId}>
              <span className="meta-label">{definition?.type ?? "Asset"}</span>
              <h3>{definition?.name ?? asset.definitionId}</h3>
              <p>{definition?.summary ?? "Catalog definition unavailable."}</p>
              {asset.companionName ? <Badge tone="info">{asset.companionName}</Badge> : null}
            </Card>
          );
        })
      )}
    </section>
  );
}

function BondsReadOnlyView({ character }: { character: CharacterDto }) {
  return (
    <section className="card-grid">
      {character.bonds.length === 0 ? (
        <Notice tone="info">No bonds recorded.</Notice>
      ) : (
        character.bonds.map((bond) => (
          <Card key={bond.id}>
            <h3>{bond.name}</h3>
            <p>{bond.description || bond.notes || "No notes recorded."}</p>
            <Badge tone="info">{bond.status}</Badge>
          </Card>
        ))
      )}
    </section>
  );
}

function VowsReadOnlyView({ character }: { character: CharacterDto }) {
  return (
    <section className="card-grid">
      {character.vows.length === 0 ? (
        <Notice tone="warning">No active vows recorded.</Notice>
      ) : (
        character.vows.map((vow) => {
          const track = character.progressTracks.find((candidate) => candidate.id === vow.progressTrackId);
          return (
            <Card key={vow.id}>
              <div className="card-title-row">
                <h3>{vow.title || "Untitled vow"}</h3>
                <Badge tone={vow.status === "active" ? "success" : "info"}>{vow.status}</Badge>
              </div>
              <p>{formatLabel(vow.rank)} vow</p>
              {track ? <Meter label="Progress" value={(track.ticks / track.maxTicks) * 100} /> : null}
            </Card>
          );
        })
      )}
    </section>
  );
}

function ConditionsReadOnlyView({ character, ruleset }: { character: CharacterDto; ruleset: RulesetDto | null }) {
  const marked = character.debilities.filter((debility) => debility.marked);
  const definitions = new Map((ruleset?.debilities ?? []).map((definition) => [definition.id, definition]));

  return (
    <div className="overview-layout">
      <Panel title="Meters" eyebrow="Conditions">
        <div className="meter-grid">
          <MeterCard label="Health" meter={character.conditionMeters.health} />
          <MeterCard label="Spirit" meter={character.conditionMeters.spirit} />
          <MeterCard label="Supply" meter={character.conditionMeters.supply} />
        </div>
      </Panel>
      <Panel title="Momentum" eyebrow="Derived">
        <DefinitionList
          items={[
            ["Current", character.momentum.current.toString()],
            ["Max", character.momentum.max.toString()],
            ["Reset", character.momentum.reset.toString()],
            ["Marked debilities", character.momentum.markedDebilityCount.toString()]
          ]}
        />
      </Panel>
      <Panel title="Debilities" eyebrow="State">
        {marked.length === 0 ? (
          <Notice tone="success" icon={<CircleCheck />}>No debilities marked.</Notice>
        ) : (
          <div className="chip-row">
            {marked.map((debility) => {
              const definition = definitions.get(debility.definitionId);
              return <Badge key={debility.definitionId} tone="warning">{definition?.name ?? debility.definitionId}</Badge>;
            })}
          </div>
        )}
      </Panel>
    </div>
  );
}

function SettingsView({
  health,
  onRefresh,
  ruleset
}: {
  health: HealthDto | null;
  onRefresh: () => void;
  ruleset: RulesetDto | null;
}) {
  return (
    <section className="screen-stack">
      <PageHeader
        eyebrow="Settings"
        title="Settings"
        actions={
          <Button variant="secondary" icon={<RefreshCw />} onClick={() => void onRefresh()}>
            Refresh
          </Button>
        }
      >
        <p>Local backend status, rules content versions, and attribution placeholders.</p>
      </PageHeader>
      <div className="summary-grid">
        <Panel title="Backend" eyebrow="Local service">
          <DefinitionList
            items={[
              ["Service", health?.service ?? "Unavailable"],
              ["Version", health?.version ?? "Unknown"],
              ["API", "/api/v1"]
            ]}
          />
        </Panel>
        <Panel title="Rules Content" eyebrow="Ironsworn">
          <DefinitionList
            items={[
              ["Ruleset", ruleset?.name ?? "Unavailable"],
              ["Ruleset version", ruleset?.rulesetVersion ?? "Unknown"],
              ["Content version", ruleset?.contentVersion ?? "Unknown"],
              ["Licensing", ruleset?.licensingStatus ?? "Pending"]
            ]}
          />
        </Panel>
      </div>
      <Notice tone="warning" icon={<AlertTriangle />}>
        Attribution and licensing text is pending product/legal approval. This UI does not display full official move or asset text.
      </Notice>
      <Notice tone="info" icon={<FileText />}>
        TODO: Sheet projection endpoint is not implemented yet; printable sheet architecture remains deferred.
      </Notice>
    </section>
  );
}

function PageHeader({
  actions,
  children,
  eyebrow,
  title
}: {
  actions?: ReactNode;
  children?: ReactNode;
  eyebrow: string;
  title: string;
}) {
  return (
    <header className="page-header">
      <div>
        <span className="meta-label">{eyebrow}</span>
        <h1>{title}</h1>
        {children}
      </div>
      {actions ? <div className="page-actions">{actions}</div> : null}
    </header>
  );
}

function StepHeading({ text, title }: { text: string; title: string }) {
  return (
    <header className="step-heading">
      <h2>{title}</h2>
      <p>{text}</p>
    </header>
  );
}

function DefinitionList({ compact = false, items }: { compact?: boolean; items: Array<[string, string]> }) {
  return (
    <dl className={compact ? "definition-list compact" : "definition-list"}>
      {items.map(([term, value]) => (
        <div key={term}>
          <dt>{term}</dt>
          <dd>{value}</dd>
        </div>
      ))}
    </dl>
  );
}

function MeterCard({ label, meter }: { label: string; meter: { current: number; min: number; max: number; isLocked: boolean } }) {
  return (
    <Card>
      <span className="meta-label">{label}</span>
      <strong className="big-value">{meter.current}</strong>
      <Meter label={`${label} meter`} value={(meter.current / meter.max) * 100} />
      {meter.isLocked ? <Badge tone="warning">Recovery locked</Badge> : null}
    </Card>
  );
}

function ProgressList({ tracks, vows }: { tracks: ProgressTrackDto[]; vows: VowDto[] }) {
  if (tracks.length === 0) {
    return <Notice tone="warning">No progress tracks recorded.</Notice>;
  }

  return (
    <div className="form-stack">
      {tracks.map((track) => {
        const vow = vows.find((candidate) => candidate.progressTrackId === track.id);
        return (
          <Card key={track.id}>
            <div className="card-title-row">
              <h3>{vow?.title || track.name}</h3>
              <Badge tone="info">{formatLabel(track.rank || track.type)}</Badge>
            </div>
            <Meter label={`${track.ticks} of ${track.maxTicks} ticks`} value={(track.ticks / track.maxTicks) * 100} />
          </Card>
        );
      })}
    </div>
  );
}

function ValidationPanel({
  issues,
  onStepChange
}: {
  issues: ValidationIssueDto[];
  onStepChange: (step: WizardStepId) => void;
}) {
  if (issues.length === 0) {
    return (
      <Notice tone="success" icon={<CircleCheck />}>
        No backend validation issues are currently reported.
      </Notice>
    );
  }

  return (
    <Panel title="Validation" eyebrow="Review">
      <ul className="validation-list">
        {issues.map((issue) => (
          <ValidationIssueRow issue={issue} key={`${issue.code}-${issue.field}`} onStepChange={onStepChange} />
        ))}
      </ul>
    </Panel>
  );
}

function ValidationIssueRow({
  issue,
  onStepChange
}: {
  issue: ValidationIssueDto;
  onStepChange: (step: WizardStepId) => void;
}) {
  const step = toWizardStep(issue.step);

  return (
    <li>
      <Badge tone={issue.severity === "error" ? "danger" : "warning"}>{issue.severity}</Badge>
      <span>{issue.message}</span>
      {step ? (
        <Button variant="secondary" onClick={() => onStepChange(step)}>
          Open {formatLabel(step)}
        </Button>
      ) : null}
    </li>
  );
}

function InfoIcon() {
  return <Sparkle aria-hidden="true" />;
}

function buildChecklist(character: CharacterDto, ruleset: RulesetDto | null) {
  const requiredAssets = ruleset?.startingCharacter.requiredAssetCount ?? 3;
  const activeVowCount = character.vows.filter((vow) => vow.status === "active" && vow.title && vow.rank).length;
  const statValues = Object.values(character.stats).sort((a, b) => b - a).join(",");
  const expectedStats = (ruleset?.startingCharacter.statArray ?? [3, 2, 2, 1, 1]).sort((a, b) => b - a).join(",");

  return [
    { label: "Name is recorded", complete: Boolean(character.name.trim()), step: "concept" as WizardStepId },
    { label: "Stats use the standard array", complete: statValues === expectedStats, step: "stats" as WizardStepId },
    { label: `Exactly ${requiredAssets} assets selected`, complete: character.assets.length === requiredAssets, step: "assets" as WizardStepId },
    { label: "Background bonds are within the starting limit", complete: character.bonds.length <= (ruleset?.startingCharacter.maxStartingBonds ?? 3), step: "bonds" as WizardStepId },
    { label: "At least one active vow has title and rank", complete: activeVowCount >= 1, step: "vows" as WizardStepId }
  ];
}

function createBond(): BondDto {
  return {
    id: createId(),
    name: "",
    type: "",
    description: "",
    location: "",
    status: "active",
    notes: "",
    createdFromBackground: true,
    createdAt: new Date().toISOString()
  };
}

function createVow(kind: "background" | "inciting"): { progressTrack: ProgressTrackDto; vow: VowDto } {
  const vowId = createId();
  const trackId = createId();
  const now = new Date().toISOString();
  const name = kind === "background" ? "Background vow" : "Inciting vow";

  return {
    progressTrack: {
      id: trackId,
      name,
      type: "vow",
      rank: "",
      ticks: 0,
      maxTicks: 40,
      progressScore: 0,
      status: "active",
      sharedScope: "character",
      notes: ""
    },
    vow: {
      id: vowId,
      title: "",
      description: "",
      rank: "",
      status: "active",
      progressTrackId: trackId,
      isBackgroundVow: kind === "background",
      isIncitingIncident: kind === "inciting",
      experienceAwarded: 0,
      notes: "",
      createdAt: now,
      completedAt: null
    }
  };
}

function createId() {
  return crypto.randomUUID ? crypto.randomUUID() : `${Date.now()}-${Math.random().toString(16).slice(2)}`;
}

function validationIssues(validation: ValidationResultDto | null) {
  return validation ? [...validation.errors, ...validation.warnings] : [];
}

function groupIssuesByField(issues: ValidationIssueDto[]) {
  const grouped = new Map<string, ValidationIssueDto[]>();
  for (const issue of issues) {
    grouped.set(issue.field, [...(grouped.get(issue.field) ?? []), issue]);
  }
  return grouped;
}

function groupIssuesByStep(issues: ValidationIssueDto[]) {
  const grouped = new Map<string, ValidationIssueDto[]>();
  for (const issue of issues) {
    grouped.set(issue.step, [...(grouped.get(issue.step) ?? []), issue]);
  }
  return grouped;
}

function firstIssue(issueByField: Map<string, ValidationIssueDto[]>, field: string) {
  return issueByField.get(field)?.[0]?.message;
}

function displayCharacterName(character: CharacterDto) {
  return character.name.trim() || "Untitled draft";
}

function formatDate(value: string | null) {
  if (!value) {
    return "Unknown";
  }

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}

function formatLabel(value: string) {
  return value
    .replace(/-/g, " ")
    .replace(/\b\w/g, (letter) => letter.toUpperCase());
}

function tabLabelToSubView(label: string): CharacterSubView {
  switch (label) {
    case "Assets":
      return "assets";
    case "Bonds":
      return "bonds";
    case "Vows":
      return "vows";
    case "Conditions":
      return "conditions";
    default:
      return "overview";
  }
}

function isWizardStep(step: string): step is WizardStepId {
  return wizardSteps.some((candidate) => candidate.id === step);
}

function toWizardStep(step?: string): WizardStepId | null {
  return step && isWizardStep(step) ? step : null;
}

function toErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    return `${error.message} (${error.code})`;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Unknown error";
}
