import type {
  ButtonHTMLAttributes,
  InputHTMLAttributes,
  ReactNode,
  SelectHTMLAttributes,
  TextareaHTMLAttributes
} from "react";

type Tone = "success" | "warning" | "danger" | "info";

function joinClassNames(...classNames: Array<string | false | null | undefined>) {
  return classNames.filter(Boolean).join(" ");
}

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  icon?: ReactNode;
  variant?: "primary" | "secondary" | "accent" | "danger";
};

export function Button({ children, className, icon, variant = "primary", ...props }: ButtonProps) {
  return (
    <button className={joinClassNames("ui-button", `ui-button--${variant}`, className)} {...props}>
      {icon ? <span className="ui-button__icon">{icon}</span> : null}
      <span>{children}</span>
    </button>
  );
}

type CardProps = {
  children: ReactNode;
  className?: string;
  elevated?: boolean;
  title?: string;
};

export function Card({ children, className, elevated = false, title }: CardProps) {
  return (
    <article className={joinClassNames("ui-card", elevated && "ui-card--elevated", className)}>
      {title ? <h3>{title}</h3> : null}
      {children}
    </article>
  );
}

type PanelProps = {
  children: ReactNode;
  className?: string;
  eyebrow?: string;
  title?: string;
};

export function Panel({ children, className, eyebrow, title }: PanelProps) {
  return (
    <section className={joinClassNames("ui-panel", className)}>
      {title ? (
        <header className="ui-panel__header">
          {eyebrow ? <span>{eyebrow}</span> : null}
          <h3>{title}</h3>
        </header>
      ) : null}
      <div className="ui-panel__body">{children}</div>
    </section>
  );
}

type FieldProps = {
  children: ReactNode;
  helpText?: string;
  label: string;
};

export function Field({ children, helpText, label }: FieldProps) {
  return (
    <label className="ui-field">
      <span className="ui-field__label">{label}</span>
      {children}
      {helpText ? <span className="ui-field__help">{helpText}</span> : null}
    </label>
  );
}

export function Input({ className, ...props }: InputHTMLAttributes<HTMLInputElement>) {
  return <input className={joinClassNames("ui-input", className)} {...props} />;
}

export function Textarea({ className, ...props }: TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea className={joinClassNames("ui-input", "ui-textarea", className)} {...props} />;
}

type SelectProps = SelectHTMLAttributes<HTMLSelectElement> & {
  icon?: ReactNode;
};

export function Select({ children, className, icon, ...props }: SelectProps) {
  return (
    <span className="ui-select">
      <select className={joinClassNames("ui-input", className)} {...props}>
        {children}
      </select>
      {icon ? <span className="ui-select__icon">{icon}</span> : null}
    </span>
  );
}

type CheckboxProps = Omit<InputHTMLAttributes<HTMLInputElement>, "type"> & {
  label: string;
};

export function Checkbox({ className, label, ...props }: CheckboxProps) {
  return (
    <label className={joinClassNames("ui-choice", className)}>
      <input type="checkbox" {...props} />
      <span className="ui-choice__box" aria-hidden="true" />
      <span>{label}</span>
    </label>
  );
}

type RadioProps = Omit<InputHTMLAttributes<HTMLInputElement>, "type"> & {
  label: string;
};

export function Radio({ className, label, ...props }: RadioProps) {
  return (
    <label className={joinClassNames("ui-choice", className)}>
      <input type="radio" {...props} />
      <span className="ui-choice__radio" aria-hidden="true" />
      <span>{label}</span>
    </label>
  );
}

type TabsProps = {
  tabs: Array<{ active?: boolean; label: string }>;
};

export function Tabs({ tabs }: TabsProps) {
  return (
    <div className="ui-tabs" role="tablist" aria-label="Demo tabs">
      {tabs.map((tab) => (
        <button
          aria-selected={tab.active ? "true" : "false"}
          className={joinClassNames("ui-tab", tab.active && "ui-tab--active")}
          key={tab.label}
          role="tab"
          tabIndex={tab.active ? 0 : -1}
          type="button"
        >
          {tab.label}
        </button>
      ))}
    </div>
  );
}

type ModalSurfaceProps = {
  children: ReactNode;
  icon?: ReactNode;
  title: string;
};

export function ModalSurface({ children, icon, title }: ModalSurfaceProps) {
  return (
    <div className="ui-modalSurface" role="dialog" aria-label={title}>
      <header>
        {icon ? <span>{icon}</span> : null}
        <h3>{title}</h3>
      </header>
      <div>{children}</div>
    </div>
  );
}

export function TooltipSurface({ children }: { children: ReactNode }) {
  return (
    <div className="ui-tooltipSurface" role="tooltip">
      {children}
    </div>
  );
}

type SidebarItemProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  active?: boolean;
  icon?: ReactNode;
};

export function SidebarItem({ active = false, children, className, icon, ...props }: SidebarItemProps) {
  return (
    <button
      aria-current={active ? "page" : undefined}
      className={joinClassNames("ui-sidebarItem", active && "ui-sidebarItem--active", className)}
      type="button"
      {...props}
    >
      {icon ? <span>{icon}</span> : null}
      <span>{children}</span>
    </button>
  );
}

type BadgeProps = {
  children: ReactNode;
  icon?: ReactNode;
  tone?: Tone;
};

export function Badge({ children, icon, tone = "info" }: BadgeProps) {
  return (
    <span className={joinClassNames("ui-badge", `ui-badge--${tone}`)}>
      {icon ? <span>{icon}</span> : null}
      {children}
    </span>
  );
}

type MeterProps = {
  label: string;
  value: number;
};

export function Meter({ label, value }: MeterProps) {
  const normalizedValue = Math.max(0, Math.min(100, value));

  return (
    <div className="ui-meter">
      <div className="ui-meter__header">
        <span>{label}</span>
        <span>{normalizedValue}%</span>
      </div>
      <div className="ui-meter__track" role="meter" aria-label={label} aria-valuemin={0} aria-valuemax={100} aria-valuenow={normalizedValue}>
        <span style={{ width: `${normalizedValue}%` }} />
      </div>
    </div>
  );
}
