import { apiRequest } from "./client";
import type { HealthDto, RulesetDto, RulesetListDto } from "./contracts";

export function getHealth() {
  return apiRequest<HealthDto>("/health");
}

export function listRulesets() {
  return apiRequest<RulesetListDto>("/api/v1/rulesets");
}

export function getRuleset(rulesetId: string) {
  return apiRequest<RulesetDto>(`/api/v1/rulesets/${encodeURIComponent(rulesetId)}`);
}
