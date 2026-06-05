import { apiMutation, apiRequest } from "./client";
import type {
  CharacterDto,
  CharacterListDto,
  CreateDraftRequest,
  FinalizeDraftRequest,
  ValidationResultDto
} from "./contracts";

export function listCharacters() {
  return apiRequest<CharacterListDto>("/api/v1/characters");
}

export function createDraft(request: CreateDraftRequest = { rulesetId: "ironsworn" }) {
  return apiMutation<CharacterDto>("/api/v1/characters/drafts", "POST", request);
}

export function getCharacter(characterId: string) {
  return apiRequest<CharacterDto>(`/api/v1/characters/${encodeURIComponent(characterId)}`);
}

export function updateCharacter(character: CharacterDto) {
  return apiMutation<CharacterDto>(
    `/api/v1/characters/${encodeURIComponent(character.id)}`,
    "PUT",
    character
  );
}

export function validateCharacter(characterId: string) {
  return apiMutation<ValidationResultDto>(
    `/api/v1/characters/${encodeURIComponent(characterId)}/validate`,
    "POST"
  );
}

export function finalizeCharacter(characterId: string, request: FinalizeDraftRequest = { standardRules: true }) {
  return apiMutation<CharacterDto>(
    `/api/v1/characters/${encodeURIComponent(characterId)}/finalize`,
    "POST",
    request
  );
}

export function deleteCharacter(characterId: string) {
  return apiMutation<void>(`/api/v1/characters/${encodeURIComponent(characterId)}`, "DELETE");
}
