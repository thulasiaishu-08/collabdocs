const BASE = '/api';

function token() {
  return localStorage.getItem('ce_token');
}

async function req(method, path, body, isForm = false) {
  const headers = {};
  if (!isForm) headers['Content-Type'] = 'application/json';
  const t = token();
  if (t) headers['Authorization'] = `Bearer ${t}`;

  const res = await fetch(`${BASE}${path}`, {
    method,
    headers,
    body: isForm ? body : body != null ? JSON.stringify(body) : undefined,
  });

  if (res.status === 204) return null;

  const text = await res.text();
  if (!res.ok) throw new Error(text || `HTTP ${res.status}`);

  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

export const api = {
  // Auth
  login: (username, password) => req('POST', '/auth/login', { username, password }),

  // Users
  getUsers: () => req('GET', '/users'),

  // My documents
  getMyDocuments:     ()          => req('GET',    '/documents'),
  getSharedDocuments: ()          => req('GET',    '/documents/shared'),
  getDocument:        (id)        => req('GET',    `/documents/${id}`),
  createDocument:     (title, content = '') =>
                                     req('POST',   '/documents', { title, content }),
  updateDocument:     (id, content) => req('PUT',  `/documents/${id}`, { content }),
  renameDocument:     (id, title)   => req('PATCH', `/documents/${id}/rename`, { title }),
  deleteDocument:     (id)          => req('DELETE', `/documents/${id}`),

  uploadDocument: (file) => {
    const form = new FormData();
    form.append('file', file);
    return req('POST', '/documents/upload', form, true);
  },

  // Shares
  getDocumentShares: (docId) => req('GET',    `/documents/${docId}/shares`),
  shareDocument:     (documentId, sharedWithUserId) =>
                               req('POST',   '/shares', { documentId, sharedWithUserId }),
  removeShare:       (id)    => req('DELETE', `/shares/${id}`),
};
