import { useState, useEffect, useRef } from 'react';

export default function RenameModal({ currentTitle, onSave, onClose }) {
  const [title, setTitle] = useState(currentTitle);
  const [error, setError] = useState('');
  const inputRef = useRef(null);

  useEffect(() => {
    inputRef.current?.select();
  }, []);

  function handleSubmit(e) {
    e.preventDefault();
    const trimmed = title.trim();
    if (!trimmed)             { setError('Title is required.');                    return; }
    if (trimmed.length > 200) { setError('Title must be 200 characters or fewer.'); return; }
    onSave(trimmed);
  }

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal" onClick={e => e.stopPropagation()}>
        <div className="modal-header">
          <h3>Rename Document</h3>
          <button className="modal-close" onClick={onClose}>✕</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="field">
            <label htmlFor="rename-input">New title</label>
            <input
              id="rename-input"
              ref={inputRef}
              type="text"
              value={title}
              onChange={e => { setTitle(e.target.value); setError(''); }}
              maxLength={200}
              required
            />
            {error && <p className="field-error">{error}</p>}
          </div>
          <div className="modal-footer">
            <button type="button" className="btn-secondary" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn-primary">Save</button>
          </div>
        </form>
      </div>
    </div>
  );
}
