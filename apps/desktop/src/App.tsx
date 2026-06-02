import { useEffect, useState } from "react";
import { Activity, Cloud, Dice5, ShieldCheck } from "lucide-react";

type HealthState =
  | { status: "loading" }
  | { status: "ready"; service: string; version: string }
  | { status: "error"; message: string };

async function loadHealth(): Promise<HealthState> {
  const baseUrl = window.characterManager
    ? await window.characterManager.getBackendBaseUrl()
    : "http://127.0.0.1:53987";

  const response = await fetch(`${baseUrl}/health`);
  if (!response.ok) {
    return { status: "error", message: `Backend returned HTTP ${response.status}` };
  }

  const payload = (await response.json()) as { service: string; version: string };
  return { status: "ready", service: payload.service, version: payload.version };
}

export function App() {
  const [health, setHealth] = useState<HealthState>({ status: "loading" });

  useEffect(() => {
    let cancelled = false;

    loadHealth()
      .then((state) => {
        if (!cancelled) {
          setHealth(state);
        }
      })
      .catch((error: unknown) => {
        if (!cancelled) {
          setHealth({
            status: "error",
            message: error instanceof Error ? error.message : "Unknown backend error"
          });
        }
      });

    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <main className="shell">
      <aside className="sidebar" aria-label="Primary navigation">
        <div className="brand">
          <Dice5 aria-hidden="true" />
          <span>CharacterManager</span>
        </div>
        <nav>
          <button className="navItem active" type="button">
            <Activity aria-hidden="true" />
            Dashboard
          </button>
          <button className="navItem" type="button">
            <ShieldCheck aria-hidden="true" />
            Ironsworn
          </button>
          <button className="navItem" type="button">
            <Cloud aria-hidden="true" />
            Atlas
          </button>
        </nav>
      </aside>

      <section className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">Ironsworn starter workspace</p>
            <h1>Character operations</h1>
          </div>
          <div className={`statusPill ${health.status}`}>
            {health.status === "ready" ? "Backend online" : health.status}
          </div>
        </header>

        <section className="panelGrid" aria-label="Application status">
          <article className="panel">
            <h2>Rules engine</h2>
            <p>Schema-driven rulesets with .NET extension points for game-specific behavior.</p>
          </article>
          <article className="panel">
            <h2>Cloud data</h2>
            <p>Atlas adapters are isolated in infrastructure so global rules and assets can evolve safely.</p>
          </article>
          <article className="panel">
            <h2>Portable release</h2>
            <p>Electron will bundle the published .NET backend into the Windows portable executable.</p>
          </article>
        </section>

        <section className="backendBox" aria-label="Backend health">
          <h2>Backend health</h2>
          {health.status === "ready" && (
            <p>
              Connected to {health.service} version {health.version}.
            </p>
          )}
          {health.status === "loading" && <p>Checking local service...</p>}
          {health.status === "error" && <p>{health.message}</p>}
        </section>
      </section>
    </main>
  );
}
