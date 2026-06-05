import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import React from "react";
import { describe, expect, it, vi } from "vitest";
import {
  Button,
  Field,
  FieldError,
  Input,
  Notice,
  SidebarItem,
  Tabs
} from "../../../apps/desktop/src/design-system";

describe("Lena design-system primitives", () => {
  it("exposes pressed state for action buttons", () => {
    render(<Button pressed>Save Draft</Button>);

    const button = screen.getByRole("button", { name: "Save Draft" });
    expect(button).toHaveAttribute("aria-pressed", "true");
    expect(button).toHaveClass("ui-button--pressed");
  });

  it("calls tab selection and marks the active tab", async () => {
    const user = userEvent.setup();
    const onSelect = vi.fn();

    render(
      <Tabs
        onSelect={onSelect}
        tabs={[
          { label: "Overview", active: true },
          { label: "Assets" }
        ]}
      />
    );

    expect(screen.getByRole("tab", { name: "Overview" })).toHaveAttribute("aria-selected", "true");
    await user.click(screen.getByRole("tab", { name: "Assets" }));
    expect(onSelect).toHaveBeenCalledWith("Assets");
  });

  it("marks sidebar items as the current page", () => {
    render(<SidebarItem active>Characters</SidebarItem>);

    expect(screen.getByRole("button", { name: "Characters" })).toHaveAttribute("aria-current", "page");
  });

  it("renders field labels, field errors, and notice roles accessibly", () => {
    render(
      <>
        <Field label="Name">
          <Input />
          <FieldError>Name is required before finalizing.</FieldError>
        </Field>
        <Notice tone="danger">Backend unavailable.</Notice>
      </>
    );

    expect(screen.getByRole("textbox")).toBeInTheDocument();
    expect(screen.getByText("Name is required before finalizing.")).toHaveClass("ui-field__error");
    expect(screen.getByRole("alert")).toHaveTextContent("Backend unavailable.");
  });
});
