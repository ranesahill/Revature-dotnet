export default function StatusBadge({ status }) {
  const key = status?.toLowerCase().replace(/\s/g, '') || 'draft';
  return (
    <span className={`status-badge ${key}`}>
      <span className="status-dot"></span>
      {status}
    </span>
  );
}
