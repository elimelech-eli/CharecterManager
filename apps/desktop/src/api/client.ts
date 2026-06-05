import type { ErrorEnvelopeDto, ValidationResultDto } from "./contracts";

const DEFAULT_BACKEND_BASE_URL = "http://127.0.0.1:53987";

export class ApiError extends Error {
  code: string;
  details: unknown[];
  status: number;
  target: string | null;

  constructor(status: number, envelope: ErrorEnvelopeDto) {
    super(envelope.error.message);
    this.name = "ApiError";
    this.status = status;
    this.code = envelope.error.code;
    this.target = envelope.error.target;
    this.details = envelope.error.details;
  }
}

export class ValidationApiError extends Error {
  status: number;
  validation: ValidationResultDto;

  constructor(status: number, validation: ValidationResultDto) {
    super("Backend validation failed.");
    this.name = "ValidationApiError";
    this.status = status;
    this.validation = validation;
  }
}

let cachedBaseUrl: string | null = null;

export async function getBackendBaseUrl() {
  if (cachedBaseUrl) {
    return cachedBaseUrl;
  }

  cachedBaseUrl = window.characterManager
    ? await window.characterManager.getBackendBaseUrl()
    : DEFAULT_BACKEND_BASE_URL;

  return cachedBaseUrl;
}

export async function apiRequest<T>(path: string, init?: RequestInit): Promise<T> {
  const baseUrl = await getBackendBaseUrl();
  const response = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {})
    }
  });

  if (!response.ok) {
    const payload = (await safeJson(response)) as ErrorEnvelopeDto | ValidationResultDto | null;
    if (isValidationResult(payload)) {
      throw new ValidationApiError(response.status, payload);
    }

    if (payload && "error" in payload && payload.error) {
      throw new ApiError(response.status, payload);
    }

    throw new Error(`Backend returned HTTP ${response.status}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export async function apiMutation<T>(path: string, method: "POST" | "PUT" | "DELETE", body?: unknown) {
  return apiRequest<T>(path, {
    method,
    body: body === undefined ? undefined : JSON.stringify(body)
  });
}

async function safeJson(response: Response) {
  try {
    return await response.json();
  } catch {
    return null;
  }
}

function isValidationResult(payload: unknown): payload is ValidationResultDto {
  return Boolean(
    payload &&
      typeof payload === "object" &&
      "isValid" in payload &&
      "errors" in payload &&
      "warnings" in payload
  );
}
