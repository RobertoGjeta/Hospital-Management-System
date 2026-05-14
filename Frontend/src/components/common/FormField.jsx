import './FormField.css';

export default function FormField({
  label,
  name,
  type = 'text',
  value,
  onChange,
  required,
  options,
  placeholder,
  rows,
  min,
  max,
  step,
  hint,
  error,
  disabled,
}) {
  const id = `field-${name}`;
  const inputProps = {
    id,
    name,
    value: value ?? '',
    onChange: (e) => onChange(e.target.value),
    placeholder,
    disabled,
    required,
  };

  return (
    <div className={`form-field${error ? ' has-error' : ''}`}>
      <label className="form-label" htmlFor={id}>
        {label}
        {required && <span className="form-required">*</span>}
      </label>

      {type === 'textarea' ? (
        <textarea {...inputProps} rows={rows ?? 3} className="form-input" />
      ) : type === 'select' ? (
        <select {...inputProps} className="form-input">
          <option value="">Select…</option>
          {options.map((o) => (
            <option key={o.value} value={o.value}>{o.label}</option>
          ))}
        </select>
      ) : (
        <input
          {...inputProps}
          type={type}
          min={min}
          max={max}
          step={step}
          className="form-input"
        />
      )}

      {hint && !error && <span className="form-hint">{hint}</span>}
      {error && <span className="form-error">{error}</span>}
    </div>
  );
}
