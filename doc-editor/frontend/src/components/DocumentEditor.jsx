import { useState, useEffect, useRef, useCallback } from 'react';
import ReactQuill from 'react-quill-new';
import 'react-quill-new/dist/quill.snow.css';
import { api } from '../services/api';
import { useAuth } from '../context/AuthContext';
import RenameModal from './RenameModal';
import ShareModal from './ShareModal';

const AUTOSAVE_DELAY = 1500;

const TOOLBAR = [
  [{ header: [1, 2, 3, false] }],
  ['bold', 'italic', 'underline', 'strike'],
  [{ color: [] }, { background: [] }],
  [{ list: 'ordered' }, { list: 'bullet' }],
  [{ indent: '-1' }, { indent: '+1' }],
  ['blockquote', 'code-block'],
  ['link'],
  ['clean'],
];

export default function DocumentEditor({ docId, onRenamed, onDeleted }) {
  const { user } = useAuth();
  const [doc,        setDoc]        = useState(null);
  const [content,    setContent]    = useState('');
  const [saveStatus, setSaveStatus] = useState('saved'); // 'saved' | 'saving' | 'unsaved'
  const [loading,    setLoading]    = useState(true);
  const [error,      setError]      = useState('');
  const [showRename, setShowRename] = useState(false);
  const [showShare,  setShowShare]  = useState(false);

  const saveTimer = useRef(null);
  const isOwner   = doc?.ownerId === user?.id;

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError('');
    api.getDocument(docId)
      .then(d => {
        if (!cancelled) {
          setDoc(d);
          setContent(d.content);
          setSaveStatus('saved');
          setLoading(false);
        }
      })
      .catch(err => {
        if (!cancelled) { setError(err.message); setLoading(false); }
      });
    return () => { cancelled = true; };
  }, [docId]);

  const save = useCallback(async (html) => {
    setSaveStatus('saving');
    try {
      const updated = await api.updateDocument(docId, html);
      setDoc(prev => prev ? { ...prev, updatedAt: updated.updatedAt } : prev);
      setSaveStatus('saved');
    } catch {
      setSaveStatus('unsaved');
    }
  }, [docId]);

  function handleChange(html) {
    setContent(html);
    setSaveStatus('unsaved');
    clearTimeout(saveTimer.current);
    saveTimer.current = setTimeout(() => save(html), AUTOSAVE_DELAY);
  }

  async function handleRename(title) {
    try {
      const updated = await api.renameDocument(docId, title);
      setDoc(prev => prev ? { ...prev, title: updated.title } : prev);
      setShowRename(false);
      onRenamed?.();
    } catch (err) {
      alert(err.message);
    }
  }

  async function handleDelete() {
    if (!window.confirm(`Delete "${doc?.title}"? This cannot be undone.`)) return;
    try {
      await api.deleteDocument(docId);
      onDeleted?.();
    } catch (err) {
      alert(err.message);
    }
  }

  function formatSaveTime(isoStr) {
    if (!isoStr) return '';
    const d = new Date(isoStr);
    return d.toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' });
  }

  if (loading) return <div className="editor-loading">Loading…</div>;
  if (error)   return <div className="editor-error">Error: {error}</div>;
  if (!doc)    return <div className="editor-error">Document not found.</div>;

  return (
    <div className="document-editor">
      <div className="editor-topbar">
        <div className="editor-title-row">
          <h2
            className="editor-title"
            title={isOwner ? 'Click to rename' : undefined}
            onClick={isOwner ? () => setShowRename(true) : undefined}
            style={{ cursor: isOwner ? 'pointer' : 'default' }}
          >
            {doc.title}
            {isOwner && <span className="edit-hint">✎</span>}
          </h2>
          <span className="owner-badge">by {doc.ownerName}</span>
        </div>

        <div className="editor-actions">
          <span className={`save-indicator save-indicator--${saveStatus}`}>
            {saveStatus === 'saving'  && '● Saving…'}
            {saveStatus === 'saved'   && `✓ Saved ${formatSaveTime(doc.updatedAt)}`}
            {saveStatus === 'unsaved' && '○ Unsaved'}
          </span>

          {isOwner && (
            <>
              <button className="btn-icon" onClick={() => setShowRename(true)} title="Rename">✎</button>
              <button className="btn-share" onClick={() => setShowShare(true)}>Share</button>
              <button className="btn-danger-icon" onClick={handleDelete} title="Delete">🗑</button>
            </>
          )}
        </div>
      </div>

      <div className="quill-wrapper">
        <ReactQuill
          theme="snow"
          value={content}
          onChange={handleChange}
          modules={{ toolbar: TOOLBAR }}
          placeholder="Start writing…"
        />
      </div>

      {showRename && (
        <RenameModal
          currentTitle={doc.title}
          onSave={handleRename}
          onClose={() => setShowRename(false)}
        />
      )}

      {showShare && (
        <ShareModal
          docId={docId}
          onClose={() => setShowShare(false)}
        />
      )}
    </div>
  );
}
