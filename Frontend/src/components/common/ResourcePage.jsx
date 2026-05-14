import { useCallback, useEffect, useMemo, useState } from 'react';
import Modal from './Modal';
import FormField from './FormField';
import './ResourcePage.css';

export default function ResourcePage({
  title,
  subtitle,
  itemNoun,
  api,
  columns,
  fields,
  emptyValues,
  buildCreateDto,
  buildUpdateDto,
  searchFields = [],
  topBarActionsAfter,
}) {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState(null);
  const [search, setSearch] = useState('');

  const [editing, setEditing] = useState(null); // null | 'new' | item
  const [formValues, setFormValues] = useState(emptyValues);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState(null);

  const [deleting, setDeleting] = useState(null);
  const [deletingBusy, setDeletingBusy] = useState(false);

  const reload = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      const data = await api.getAll();
      setItems(Array.isArray(data) ? data : []);
    } catch (err) {
      setLoadError(err.message || 'Failed to load');
    } finally {
      setLoading(false);
    }
  }, [api]);

  useEffect(() => { reload(); }, [reload]);

  const visibleItems = useMemo(() => {
    if (!search.trim()) return items;
    const q = search.trim().toLowerCase();
    return items.filter((item) =>
      searchFields.some((f) => String(item[f] ?? '').toLowerCase().includes(q))
    );
  }, [items, search, searchFields]);

  const openCreate = () => {
    setEditing('new');
    setFormValues(emptyValues);
    setFormError(null);
  };

  const openEdit = (item) => {
    const initial = {};
    fields.forEach((f) => {
      const camelKey = f.name.charAt(0).toLowerCase() + f.name.slice(1);
      const raw = item[camelKey] ?? item[f.name];
      if (f.type === 'date' && raw) {
        initial[f.name] = String(raw).slice(0, 10);
      } else if (raw === null || raw === undefined) {
        initial[f.name] = '';
      } else {
        initial[f.name] = raw;
      }
    });
    setEditing(item);
    setFormValues(initial);
    setFormError(null);
  };

  const closeForm = () => {
    setEditing(null);
    setFormError(null);
  };

  const submitForm = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setFormError(null);
    try {
      if (editing === 'new') {
        const dto = buildCreateDto ? buildCreateDto(formValues) : { ...formValues };
        await api.create(dto);
      } else {
        const dto = buildUpdateDto ? buildUpdateDto(formValues, editing) : { ...formValues };
        await api.update(editing.id ?? editing.Id, dto);
      }
      closeForm();
      await reload();
    } catch (err) {
      setFormError(err.message || 'Save failed');
    } finally {
      setSubmitting(false);
    }
  };

  const confirmDelete = async () => {
    if (!deleting) return;
    setDeletingBusy(true);
    try {
      await api.delete(deleting.id ?? deleting.Id);
      setDeleting(null);
      await reload();
    } catch (err) {
      setFormError(err.message || 'Delete failed');
    } finally {
      setDeletingBusy(false);
    }
  };

  return (
    <div className="resource-page">
      <div className="resource-toolbar">
        <div className="resource-toolbar-left">
          <h2 className="resource-title">{title}</h2>
          {subtitle && <p className="resource-subtitle">{subtitle}</p>}
        </div>

        <div className="resource-toolbar-right">
          {searchFields.length > 0 && (
            <div className="search-input-wrap">
              <svg viewBox="0 0 24 24" className="search-icon"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
              <input
                type="search"
                className="search-input"
                placeholder={`Search ${itemNoun.toLowerCase()}s…`}
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </div>
          )}
          {topBarActionsAfter}
          <button className="btn btn-primary" onClick={openCreate}>
            <svg viewBox="0 0 24 24"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
            New {itemNoun}
          </button>
        </div>
      </div>

      <div className="resource-card">
        {loading ? (
          <div className="resource-state">Loading…</div>
        ) : loadError ? (
          <div className="resource-state resource-error">
            <strong>Couldn't reach the API.</strong>
            <span>{loadError}</span>
            <button className="btn btn-ghost" onClick={reload}>Retry</button>
          </div>
        ) : visibleItems.length === 0 ? (
          <div className="resource-state">
            {items.length === 0
              ? `No ${itemNoun.toLowerCase()}s yet. Click "New ${itemNoun}" to add one.`
              : `No matches for "${search}".`}
          </div>
        ) : (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  {columns.map((c) => (
                    <th key={c.key} style={c.width ? { width: c.width } : undefined}>{c.label}</th>
                  ))}
                  <th className="col-actions">Actions</th>
                </tr>
              </thead>
              <tbody>
                {visibleItems.map((item) => (
                  <tr key={item.id ?? item.Id}>
                    {columns.map((c) => (
                      <td key={c.key}>{c.render ? c.render(item) : item[c.key] ?? '—'}</td>
                    ))}
                    <td className="col-actions">
                      <button className="btn-icon" onClick={() => openEdit(item)} title="Edit">
                        <svg viewBox="0 0 24 24"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                      </button>
                      <button className="btn-icon btn-icon-danger" onClick={() => setDeleting(item)} title="Delete">
                        <svg viewBox="0 0 24 24"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/><path d="M10 11v6M14 11v6"/><path d="M9 6V4a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2v2"/></svg>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <Modal
        open={editing !== null}
        title={editing === 'new' ? `New ${itemNoun}` : `Edit ${itemNoun}`}
        onClose={closeForm}
        size="md"
        footer={
          <>
            <button className="btn btn-ghost" type="button" onClick={closeForm} disabled={submitting}>
              Cancel
            </button>
            <button className="btn btn-primary" type="submit" form="resource-form" disabled={submitting}>
              {submitting ? 'Saving…' : editing === 'new' ? `Create ${itemNoun}` : 'Save changes'}
            </button>
          </>
        }
      >
        <form id="resource-form" className="resource-form" onSubmit={submitForm}>
          {fields
            .filter((f) => editing === 'new' || !f.createOnly)
            .map((f) => (
              <FormField
                key={f.name}
                label={f.label}
                name={f.name}
                type={f.type ?? 'text'}
                options={f.options}
                value={formValues[f.name] ?? ''}
                onChange={(v) => setFormValues((s) => ({ ...s, [f.name]: v }))}
                required={f.required && editing === 'new'}
                placeholder={f.placeholder}
                rows={f.rows}
                hint={f.hint}
              />
            ))}
          {formError && <div className="form-banner-error">{formError}</div>}
        </form>
      </Modal>

      <Modal
        open={deleting !== null}
        title={`Delete ${itemNoun}?`}
        onClose={() => setDeleting(null)}
        size="sm"
        footer={
          <>
            <button className="btn btn-ghost" onClick={() => setDeleting(null)} disabled={deletingBusy}>
              Cancel
            </button>
            <button className="btn btn-danger" onClick={confirmDelete} disabled={deletingBusy}>
              {deletingBusy ? 'Deleting…' : 'Delete'}
            </button>
          </>
        }
      >
        {deleting && (
          <p className="confirm-text">
            This will permanently delete{' '}
            <strong>
              {deleting.firstName ?? deleting.FirstName} {deleting.lastName ?? deleting.LastName}
            </strong>
            . This action cannot be undone.
          </p>
        )}
      </Modal>
    </div>
  );
}
