import { useState, useRef } from 'react';
import { api } from '../services/api';

export default function UploadModal({ onClose, onUploaded }) {
  const [file,      setFile]      = useState(null);
  const [preview,   setPreview]   = useState('');
  const [uploading, setUploading] = useState(false);
  const [error,     setError]     = useState('');
  const inputRef = useRef(null);

  function handleFile(selected) {
    if (!selected) return;
    const ext = selected.name.split('.').pop()?.toLowerCase();
    if (ext !== 'txt' && ext !== 'md') {
      setError('Only .txt and .md files are supported.');
      return;
    }
    setError('');
    setFile(selected);
    const reader = new FileReader();
    reader.onload = e => setPreview(e.target.result.slice(0, 500));
    reader.readAsText(selected);
  }

  async function handleUpload() {
    if (!file) return;
    setUploading(true);
    setError('');
    try {
      const doc = await api.uploadDocument(file);
      onUploaded(doc);
    } catch (err) {
      setError(err.message);
      setUploading(false);
    }
  }

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal" onClick={e => e.stopPropagation()}>
        <div className="modal-header">
          <h3>Upload Document</h3>
          <button className="modal-close" onClick={onClose}>✕</button>
        </div>

        <div className="upload-zone" onClick={() => inputRef.current?.click()}>
          <input
            ref={inputRef}
            type="file"
            accept=".txt,.md"
            hidden
            onChange={e => handleFile(e.target.files?.[0] ?? null)}
          />
          {file ? (
            <div className="upload-chosen">
              <span className="upload-icon">📄</span>
              <span className="upload-filename">{file.name}</span>
              <span className="upload-size">({(file.size / 1024).toFixed(1)} KB)</span>
            </div>
          ) : (
            <div className="upload-placeholder">
              <span className="upload-icon">⬆</span>
              <p>Click to choose a <strong>.txt</strong> or <strong>.md</strong> file</p>
              <p className="upload-hint">Max 5 MB</p>
            </div>
          )}
        </div>

        {preview && (
          <div className="upload-preview">
            <p className="upload-preview-label">Preview</p>
            <pre>{preview}{preview.length === 500 ? '…' : ''}</pre>
          </div>
        )}

        {error && <p className="form-error">{error}</p>}

        <div className="modal-footer">
          <button className="btn-secondary" onClick={onClose}>Cancel</button>
          <button
            className="btn-primary"
            onClick={handleUpload}
            disabled={!file || uploading}
          >
            {uploading ? 'Uploading…' : 'Upload'}
          </button>
        </div>
      </div>
    </div>
  );
}
