import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import React from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { App } from "../../../apps/desktop/src/App";
import type {
  CharacterDto,
  CharacterListDto,
  HealthDto,
  RulesetDto,
  ValidationResultDto
} from "../../../apps/desktop/src/api/contracts";

const health: HealthDto = {
  service: "CharacterManager.Api",
  version: "1.0.0"
};

const ruleset: RulesetDto = {
  id: "ironsworn",
  name: "Ironsworn",
  rulesetVersion: "0.1.0",
  contentVersion: "0.1.0",
  licensingStatus: "metadataOnly",
  stats: [
    { id: "edge", name: "Edge", min: 1, max: 3, summary: "Quickness and ranged action.", sourceReference: null },
    { id: "heart", name: "Heart", min: 1, max: 3, summary: "Courage and fellowship.", sourceReference: null },
    { id: "iron", name: "Iron", min: 1, max: 3, summary: "Strength and endurance.", sourceReference: null },
    { id: "shadow", name: "Shadow", min: 1, max: 3, summary: "Stealth and deception.", sourceReference: null },
    { id: "wits", name: "Wits", min: 1, max: 3, summary: "Expertise and observation.", sourceReference: null }
  ],
  startingCharacter: {
    statArray: [3, 2, 2, 1, 1],
    health: 5,
    spirit: 5,
    supply: 5,
    momentum: 2,
    momentumMax: 10,
    momentumReset: 2,
    requiredAssetCount: 3,
    maxStartingBonds: 3,
    requiredActiveVowCount: 1
  },
  meters: [],
  momentum: { minimum: -6, baseMax: 10, resetByMarkedDebilityCount: [] },
  ranks: ["troublesome", "dangerous", "formidable", "extreme", "epic"],
  debilities: [],
  assets: [
    {
      id: "path-placeholder",
      name: "Path Placeholder",
      type: "path",
      summary: "Metadata-only placeholder for an Ironsworn path asset.",
      abilities: [],
      requiresCompanionName: false,
      permitsDuplicates: false,
      sourceReference: null,
      licensingStatus: "metadataOnly"
    }
  ]
};

function createDraft(): CharacterDto {
  return {
    id: "draft-1",
    schemaVersion: 1,
    rulesetId: "ironsworn",
    rulesetVersion: "0.1.0",
    contentVersion: "0.1.0",
    status: "draft",
    name: "",
    concept: "",
    backgroundNotes: "",
    equipmentNotes: "",
    stats: { edge: 0, heart: 0, iron: 0, shadow: 0, wits: 0 },
    conditionMeters: {
      health: { current: 5, min: 0, max: 5, isLocked: false, lockedByDebilityId: null },
      spirit: { current: 5, min: 0, max: 5, isLocked: false, lockedByDebilityId: null },
      supply: { current: 5, min: 0, max: 5, isLocked: false, lockedByDebilityId: null }
    },
    momentum: { current: 2, max: 10, reset: 2, minimum: -6, markedDebilityCount: 0 },
    debilities: [],
    assets: [],
    bonds: [],
    vows: [],
    progressTracks: [],
    experience: { earned: 0, spent: 0, available: 0 },
    createdAt: "2026-06-05T00:00:00.000Z",
    updatedAt: "2026-06-05T00:00:00.000Z"
  };
}

function jsonResponse(body: unknown, init: ResponseInit = {}) {
  return new Response(JSON.stringify(body), {
    headers: { "Content-Type": "application/json" },
    status: 200,
    ...init
  });
}

function installFetchMock({
  failHealth = false,
  list = { items: [] },
  validation
}: {
  failHealth?: boolean;
  list?: CharacterListDto;
  validation?: ValidationResultDto;
} = {}) {
  const draft = createDraft();

  vi.stubGlobal(
    "fetch",
    vi.fn(async (input: RequestInfo | URL, init?: RequestInit) => {
      const url = input.toString();
      const method = init?.method ?? "GET";

      if (url.endsWith("/health")) {
        return failHealth ? jsonResponse({ error: { code: "health.down", message: "Offline", target: null, details: [] } }, { status: 503 }) : jsonResponse(health);
      }

      if (url.endsWith("/api/v1/rulesets/ironsworn")) {
        return jsonResponse(ruleset);
      }

      if (url.endsWith("/api/v1/characters") && method === "GET") {
        return jsonResponse(list);
      }

      if (url.endsWith("/api/v1/characters/drafts") && method === "POST") {
        return jsonResponse(draft, { status: 201 });
      }

      if (url.endsWith("/api/v1/characters/draft-1") && method === "PUT") {
        return jsonResponse({ ...draft, ...(JSON.parse(String(init?.body)) as CharacterDto) });
      }

      if (url.endsWith("/api/v1/characters/draft-1/finalize") && method === "POST") {
        return jsonResponse(
          validation ?? {
            isValid: false,
            errors: [
              {
                code: "character.nameRequired",
                field: "name",
                message: "Name is required before finalizing.",
                severity: "error",
                step: "concept"
              }
            ],
            warnings: []
          },
          { status: 400 }
        );
      }

      return jsonResponse({ error: { code: "test.unhandled", message: url, target: null, details: [] } }, { status: 500 });
    })
  );
}

describe("CharacterManager app shell", () => {
  beforeEach(() => {
    vi.unstubAllGlobals();
  });

  afterEach(() => {
    vi.restoreAllMocks();
    vi.unstubAllGlobals();
  });

  it("loads the real app shell and starts a draft wizard", async () => {
    const user = userEvent.setup();
    installFetchMock();

    render(<App />);

    expect(await screen.findByRole("heading", { name: "Character Library" })).toBeInTheDocument();
    await user.click(screen.getAllByRole("button", { name: "New Character" })[0]);

    expect(await screen.findByRole("heading", { name: "Create Character" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Concept/ })).toHaveAttribute("aria-current", "step");
  });

  it("shows backend unavailable state", async () => {
    installFetchMock({ failHealth: true });

    render(<App />);

    expect(await screen.findByText("Backend unavailable or request failed.")).toBeInTheDocument();
  });

  it("renders finalize validation errors on the review step", async () => {
    const user = userEvent.setup();
    installFetchMock();

    render(<App />);

    await screen.findByRole("heading", { name: "Character Library" });
    await user.click(screen.getAllByRole("button", { name: "New Character" })[0]);
    await user.click(screen.getByRole("button", { name: /Review/ }));
    await user.click(screen.getByRole("button", { name: "Finalize" }));

    await waitFor(() => {
      expect(screen.getByText("Name is required before finalizing.")).toBeInTheDocument();
    });
    expect(screen.getByRole("button", { name: "Open Concept" })).toBeInTheDocument();
  });
});
