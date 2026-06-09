import { useState, useEffect, useCallback } from 'react';
import { api } from '../services/api';
import Sidebar from './Sidebar';
import DocumentEditor from './DocumentEditor';
import UploadModal from './UploadModal';

export default function Layout() {
  const [myDocs,     setMyDocs]     = useState([]);
  const [sharedDocs, setSharedDocs] = useState([]);
  const [activeId,   setActiveId]   = useState(null);
  const [showUpload, setShowUpload] = useState(false);

  const loadLists = useCallback(async () => {
    try {
      const [mine, shared] = await Promise.all([
        api.getMyDocuments(),
        api.getSharedDocuments(),
      ]);
      setMyDocs(mine);
      setSharedDocs(shared);
    } catch {
      // silently ignore refresh errors
    }
  }, []);

  useEffect(() => { loadLists(); }, [loadLists]);

  async function handleNew() {
    try {
      const doc = await api.createDocument('Untitled Document');
      await loadLists();
      setActiveId(doc.id);
    } catch (err) {
      alert(err.message);
    }
  }

  async function handleUploaded(doc) {
    setShowUpload(false);
    await loadLists();
    setActiveId(doc.id);
  }

  return (
    <div className="app-layout">
      <Sidebar
        myDocs={myDocs}
        sharedDocs={sharedDocs}
        activeDocId={activeId}
        onSelect={setActiveId}
        onNew={handleNew}
        onUpload={() => setShowUpload(true)}
      />

      <main className="editor-area">
        {activeId ? (
          <DocumentEditor
            key={activeId}
            docId={activeId}
            onRenamed={loadLists}
            onDeleted={() => { setActiveId(null); loadLists(); }}
          />
        ) : (
          <div className="empty-state">
            <div className="empty-icon">📝</div>
            <h2>No document open</h2>
            <p>Create a new document or select one from the sidebar.</p>
            <button className="btn-primary" onClick={handleNew}>
              New Document
            </button>
          </div>
        )}
      </main>

      {showUpload && (
        <UploadModal
          onClose={() => setShowUpload(false)}
          onUploaded={handleUploaded}
        />
      )}
    </div>
  );
}
