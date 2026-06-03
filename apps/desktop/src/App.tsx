import {
  BookOpen,
  Check,
  ChevronDown,
  CircleAlert,
  CircleCheck,
  CircleDot,
  Info,
  Library,
  Map,
  Search,
  Shield,
  Sparkle,
  SquarePen,
  Swords,
  X
} from "lucide-react";
import { useState } from "react";
import {
  Badge,
  Button,
  Card,
  Checkbox,
  Field,
  Input,
  Meter,
  ModalSurface,
  Panel,
  Radio,
  Select,
  SidebarItem,
  Tabs,
  Textarea,
  TooltipSurface
} from "./design-system";

const colorTokens = [
  ["background-primary", "#0B090F"],
  ["background-secondary", "#14111B"],
  ["surface", "#1C1824"],
  ["surface-elevated", "#262030"],
  ["purple-100", "#E7DCF8"],
  ["purple-200", "#C8B2EA"],
  ["purple-300", "#9E78D3"],
  ["purple-400", "#6F45A8"],
  ["purple-500", "#3D255F"],
  ["gold-100", "#F4E7BA"],
  ["gold-200", "#D7B96A"],
  ["gold-300", "#A98235"],
  ["gold-400", "#6F5224"],
  ["success", "#7EA66A"],
  ["warning", "#C49A4A"],
  ["danger", "#B85F56"],
  ["info", "#6F93B8"]
];

const spacingTokens = [
  ["spacing-2xs", "2px"],
  ["spacing-xs", "4px"],
  ["spacing-sm", "8px"],
  ["spacing-md", "12px"],
  ["spacing-lg", "16px"],
  ["spacing-xl", "24px"],
  ["spacing-2xl", "32px"],
  ["spacing-3xl", "48px"],
  ["spacing-4xl", "64px"]
];

const notesText = `Weathered, readable, and useful before decorative.

An iron vow begins with a clear record: who swore it, why it matters, and what progress has already been marked.

Long notes should remain calm to read and practical to search. The field keeps its dark input surface, the search affordance stays attached, and the scrollbar appears only when the text grows beyond the available height.

Use this pattern for journals, vow notes, asset notes, and compact reference text where scanning matters more than ornament.`;

export function App() {
  const [pressedButton, setPressedButton] = useState("Primary");
  const [activeTab, setActiveTab] = useState("Overview");
  const [activeSidebarItem, setActiveSidebarItem] = useState("Characters");

  return (
    <main className="ds-page">
      <header className="ds-hero">
        <div className="ds-eyebrow">Design System Demo</div>
        <h1>Ironsworn Character Manager</h1>
        <p>
          Reusable foundations for Lena's dark fantasy interface language: calm surfaces,
          readable type, antique gold emphasis, and restrained royal purple controls.
        </p>
      </header>

      <section className="ds-section">
        <div className="ds-sectionHeader">
          <div>
            <div className="ds-label">Color</div>
            <h2>Core Palette</h2>
          </div>
          <p>Exact design tokens from Lena's system, translated into CSS variables.</p>
        </div>
        <div className="ds-colorGrid">
          {colorTokens.map(([name, value]) => (
            <Card key={name} className="ds-swatch">
              <div className="ds-swatchColor" style={{ backgroundColor: value }} />
              <div className="ds-tokenBody">
                <strong>{name}</strong>
                <code>{value}</code>
              </div>
            </Card>
          ))}
        </div>
      </section>

      <section className="ds-section">
        <div className="ds-sectionHeader">
          <div>
            <div className="ds-label">Typography</div>
            <h2>Readable Mythic Type</h2>
          </div>
        </div>
        <Panel className="ds-typePanel">
          <div className="ds-typeRow">
            <span>H1 40 / 48</span>
            <div className="ds-typeSample ds-typeH1">Swear an Iron Vow</div>
          </div>
          <div className="ds-typeRow">
            <span>H2 32 / 40</span>
            <div className="ds-typeSample ds-typeH2">Character Chronicle</div>
          </div>
          <div className="ds-typeRow">
            <span>H3 24 / 32</span>
            <div className="ds-typeSample ds-typeH3">Bonds and Progress</div>
          </div>
          <div className="ds-typeRow">
            <span>Body 16 / 24</span>
            <div className="ds-typeSample">Default interface text remains crisp and useful.</div>
          </div>
          <div className="ds-typeRow">
            <span>Caption 12 / 16</span>
            <div className="ds-typeSample ds-caption">Ancient Archive</div>
          </div>
        </Panel>
      </section>

      <section className="ds-section">
        <div className="ds-sectionHeader">
          <div>
            <div className="ds-label">Spacing</div>
            <h2>8px Rhythm</h2>
          </div>
        </div>
        <Panel>
          {spacingTokens.map(([name, value]) => (
            <div className="ds-spacingRow" key={name}>
              <strong>{name}</strong>
              <span className="ds-spacingTrack">
                <span style={{ width: value }} />
              </span>
              <code>{value}</code>
            </div>
          ))}
        </Panel>
      </section>

      <section className="ds-section">
        <div className="ds-sectionHeader">
          <div>
            <div className="ds-label">Primitives</div>
            <h2>Core Components</h2>
          </div>
        </div>
        <div className="ds-componentGrid">
          <Panel title="Buttons" eyebrow="Actions">
            <div className="ds-row">
              <Button
                icon={<SquarePen />}
                onClick={() => setPressedButton("Primary")}
                pressed={pressedButton === "Primary"}
              >
                Primary
              </Button>
              <Button
                variant="secondary"
                icon={<BookOpen />}
                onClick={() => setPressedButton("Secondary")}
                pressed={pressedButton === "Secondary"}
              >
                Secondary
              </Button>
              <Button
                variant="accent"
                icon={<Sparkle />}
                onClick={() => setPressedButton("Gold Accent")}
                pressed={pressedButton === "Gold Accent"}
              >
                Gold Accent
              </Button>
              <Button
                variant="danger"
                icon={<X />}
                onClick={() => setPressedButton("Danger")}
                pressed={pressedButton === "Danger"}
              >
                Danger
              </Button>
            </div>
          </Panel>

          <Panel title="Fields" eyebrow="Forms">
            <div className="ds-formGrid">
              <Field label="Name" helpText="Standard input styling.">
                <Input defaultValue="Kara Iron-Eyes" />
              </Field>
              <Field label="Path">
                <Select defaultValue="shadow-kin" icon={<ChevronDown />}>
                  <option value="shadow-kin">Shadow-Kin</option>
                  <option value="banner">Banner-Bound</option>
                </Select>
              </Field>
              <Field label="Notes">
                <div className="ds-notesControl">
                  <span className="ds-notesSearch">
                    <Search aria-hidden="true" />
                    <Input aria-label="Search notes" placeholder="Search notes" type="search" />
                  </span>
                  <Textarea className="ds-notesTextarea" defaultValue={notesText} />
                </div>
              </Field>
            </div>
          </Panel>

          <Panel title="Selection" eyebrow="Controls">
            <div className="ds-row">
              <Checkbox defaultChecked label="Marked" />
              <Radio defaultChecked name="demo-choice" label="Chosen" />
              <Radio name="demo-choice" label="Quiet" />
            </div>
          </Panel>

          <Panel title="Tabs" eyebrow="Navigation">
            <Tabs
              onSelect={setActiveTab}
              tabs={[
                { label: "Overview", active: activeTab === "Overview" },
                { label: "Assets", active: activeTab === "Assets" },
                { label: "Vows", active: activeTab === "Vows" }
              ]}
            />
          </Panel>

          <Panel title="Sidebar Item" eyebrow="Navigation">
            <div className="ds-sidebarSample">
              <SidebarItem
                active={activeSidebarItem === "Characters"}
                icon={<Library />}
                onClick={() => setActiveSidebarItem("Characters")}
              >
                Characters
              </SidebarItem>
              <SidebarItem
                active={activeSidebarItem === "Rules"}
                icon={<Shield />}
                onClick={() => setActiveSidebarItem("Rules")}
              >
                Rules
              </SidebarItem>
              <SidebarItem
                active={activeSidebarItem === "Journal"}
                icon={<Map />}
                onClick={() => setActiveSidebarItem("Journal")}
              >
                Journal
              </SidebarItem>
            </div>
          </Panel>

          <Panel title="Surfaces" eyebrow="Layers">
            <div className="ds-surfaceStack">
              <Card title="Default Card">Ledger-like surface with subtle border.</Card>
              <Card elevated title="Elevated Card">
                Raised content uses the softer candlelit shadow.
              </Card>
            </div>
          </Panel>

          <Panel title="Status Chips" eyebrow="State">
            <div className="ds-row">
              <Badge tone="success" icon={<CircleCheck />}>
                Success
              </Badge>
              <Badge tone="warning" icon={<CircleAlert />}>
                Warning
              </Badge>
              <Badge tone="danger" icon={<Swords />}>
                Danger
              </Badge>
              <Badge tone="info" icon={<Info />}>
                Info
              </Badge>
            </div>
          </Panel>

          <Panel title="Tooltip" eyebrow="Overlay">
            <TooltipSurface>Progress is marked in ten boxes.</TooltipSurface>
          </Panel>

          <Panel title="Progress Meter" eyebrow="Visual">
            <Meter value={60} label="Vow progress" />
          </Panel>

          <Panel title="Modal Surface" eyebrow="Overlay">
            <ModalSurface title="Archive Note" icon={<CircleDot />}>
              Temporary highest layer with stronger shadow, clear border, and practical type.
              <div className="ds-modalActions">
                <Button variant="secondary">Cancel</Button>
                <Button icon={<Check />}>Confirm</Button>
              </div>
            </ModalSurface>
          </Panel>
        </div>
      </section>
    </main>
  );
}
