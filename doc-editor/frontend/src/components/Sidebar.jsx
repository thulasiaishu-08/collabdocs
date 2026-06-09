import { useAuth } from '../context/AuthContext';

function DocItem({ doc, active, onClick, isShared }) {
  const updated = new Date(doc.updatedAt);
  const label   = updated.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });

  return (
    <button
      className={`doc-item ${active ? 'doc-item--active' : ''}`}
      onClick={() => onClick(doc.id)}
      title={doc.title}
    >
      <span className="doc-item-icon">{isShared ? '👥' : '📄'}</span>
      <span className="doc-item-body">
        <span className="doc-item-title">{doc.title || 'Untitled'}</span>
        <span className="doc-item-meta">
          {isShared ? `by ${doc.ownerName}` : label}
        </span>
      </span>
    </button>
  );
}

export default function Sidebar({
  myDocs,
  sharedDocs,
  activeDocId,
  onSelect,
  onNew,
  onUpload,
}) {
  const { user, logout } = useAuth();

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <span className="sidebar-logo">📝</span>
        <span className="sidebar-brand">CollabDocs</span>
      </div>

      <div className="sidebar-actions">
        <button className="btn-new" onClick={onNew}>
          <span>＋</span> New Document
        </button>
        <button className="btn-upload" onClick={onUpload}>
          <span>⬆</span> Upload File
        </button>
      </div>

      <nav className="sidebar-nav">
        <div className="nav-section">
          <p className="nav-section-title">My Documents</p>
          {myDocs.length === 0 ? (
            <p className="nav-empty">No documents yet</p>
          ) : (
            myDocs.map(d => (
              <DocItem
                key={d.id}
                doc={d}
                active={d.id === activeDocId}
                onClick={onSelect}
                isShared={false}
              />
            ))
          )}
        </div>

        <div className="nav-section">
          <p className="nav-section-title">Shared with Me</p>
          {sharedDocs.length === 0 ? (
            <p className="nav-empty">Nothing shared yet</p>
          ) : (
            sharedDocs.map(d => (
              <DocItem
                key={d.id}
                doc={d}
                active={d.id === activeDocId}
                onClick={onSelect}
                isShared={true}
              />
            ))
          )}
        </div>
      </nav>

      <div className="sidebar-footer">
        <div className="user-pill">
          <span className="user-avatar">{user?.username?.[0]?.toUpperCase()}</span>
          <span className="user-name">{user?.username}</span>
        </div>
        <button className="btn-logout" onClick={logout} title="Sign out">⏻</button>
      </div>
    </aside>
  );
}
