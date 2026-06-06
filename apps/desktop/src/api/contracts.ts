export type CharacterStatus = "draft" | "finalized" | "invalid";
export type IssueSeverity = "error" | "warning";

export type HealthDto = {
  service: string;
  version: string;
};

export type RulesetListDto = {
  items: RulesetSummaryDto[];
};

export type RulesetSummaryDto = {
  id: string;
  name: string;
  rulesetVersion: string;
  contentVersion: string;
  licensingStatus: string;
};

export type RulesetDto = RulesetSummaryDto & {
  stats: StatDefinitionDto[];
  startingCharacter: StartingCharacterDto;
  meters: MeterDefinitionDto[];
  momentum: MomentumRulesDto;
  ranks: string[];
  debilities: DebilityDefinitionDto[];
  moves: MoveDefinitionDto[];
  assets: AssetDefinitionDto[];
};

export type StatDefinitionDto = {
  id: string;
  name: string;
  min: number;
  max: number;
  summary: string;
  sourceReference: string | null;
};

export type StartingCharacterDto = {
  statArray: number[];
  health: number;
  spirit: number;
  supply: number;
  momentum: number;
  momentumMax: number;
  momentumReset: number;
  requiredAssetCount: number;
  maxStartingBonds: number;
  requiredActiveVowCount: number;
};

export type MeterDefinitionDto = {
  id: string;
  name: string;
  minimum: number;
  maximum: number;
};

export type MomentumRulesDto = {
  minimum: number;
  baseMax: number;
  resetByMarkedDebilityCount: Array<{ markedCount: number; reset: number }>;
};

export type DebilityDefinitionDto = {
  id: string;
  name: string;
  category: string;
  blocksMeterIncrease: string | null;
  isPermanentByDefault: boolean;
  requiresLinkedVow: boolean;
  sourceReference: string | null;
};

export type MoveDefinitionDto = {
  id: string;
  name: string;
  trigger: string;
  statId: string | null;
};

export type AssetDefinitionDto = {
  id: string;
  name: string;
  type: string;
  summary: string;
  abilities: AssetAbilityDefinitionDto[];
  requiresCompanionName: boolean;
  permitsDuplicates: boolean;
  sourceReference: string | null;
  licensingStatus: string;
};

export type AssetAbilityDefinitionDto = {
  id: string;
  name: string;
  summary: string;
};

export type CharacterListDto = {
  items: CharacterSummaryDto[];
};

export type CharacterSummaryDto = {
  id: string;
  name: string | null;
  displayName: string;
  concept: string | null;
  rulesetId: string | null;
  status: CharacterStatus;
  isValid: boolean;
  needsReview: boolean;
  lastUpdatedAt: string | null;
  stateSummary: CharacterStateSummaryDto;
  error: CharacterRecordErrorDto | null;
};

export type CharacterStateSummaryDto = {
  health: number;
  spirit: number;
  supply: number;
  momentum: number;
  activeVowCount: number;
  markedDebilityCount: number;
};

export type CharacterRecordErrorDto = {
  code: string;
  message: string;
};

export type CharacterDto = {
  id: string;
  schemaVersion: number;
  rulesetId: string;
  rulesetVersion: string;
  contentVersion: string;
  status: "draft" | "finalized";
  name: string;
  concept: string;
  backgroundNotes: string;
  equipmentNotes: string;
  stats: Record<string, number>;
  conditionMeters: ConditionMetersDto;
  momentum: MomentumDto;
  debilities: DebilityStateDto[];
  assets: SelectedAssetDto[];
  bonds: BondDto[];
  vows: VowDto[];
  progressTracks: ProgressTrackDto[];
  experience: ExperienceDto;
  createdAt: string;
  updatedAt: string;
};

export type ConditionMetersDto = {
  health: MeterStateDto;
  spirit: MeterStateDto;
  supply: MeterStateDto;
};

export type MeterStateDto = {
  current: number;
  min: number;
  max: number;
  isLocked: boolean;
  lockedByDebilityId: string | null;
};

export type MomentumDto = {
  current: number;
  max: number;
  reset: number;
  minimum: number;
  markedDebilityCount: number;
};

export type DebilityStateDto = {
  definitionId: string;
  marked: boolean;
  notes: string;
  linkedVowId: string | null;
  markedAt: string | null;
  clearedAt: string | null;
};

export type SelectedAssetDto = {
  definitionId: string;
  selectedAbilityIds: string[];
  companionName: string | null;
  companionHealth: number | null;
  notes: string;
  sourceReference: string | null;
};

export type BondDto = {
  id: string;
  name: string;
  type: string;
  description: string;
  location: string;
  status: string;
  notes: string;
  createdFromBackground: boolean;
  createdAt: string;
};

export type VowDto = {
  id: string;
  title: string;
  description: string;
  rank: string;
  status: string;
  progressTrackId: string;
  isBackgroundVow: boolean;
  isIncitingIncident: boolean;
  experienceAwarded: number;
  notes: string;
  createdAt: string;
  completedAt: string | null;
};

export type ProgressTrackDto = {
  id: string;
  name: string;
  type: string;
  rank: string;
  ticks: number;
  maxTicks: number;
  progressScore: number;
  status: string;
  sharedScope: string;
  notes: string;
};

export type ExperienceDto = {
  earned: number;
  spent: number;
  available: number;
};

export type CreateDraftRequest = {
  rulesetId?: string;
};

export type FinalizeDraftRequest = {
  standardRules: boolean;
};

export type ValidationResultDto = {
  isValid: boolean;
  errors: ValidationIssueDto[];
  warnings: ValidationIssueDto[];
};

export type ValidationIssueDto = {
  code: string;
  message: string;
  field: string;
  step: string;
  severity: IssueSeverity;
};

export type ErrorEnvelopeDto = {
  error: {
    code: string;
    message: string;
    target: string | null;
    details: unknown[];
  };
};
