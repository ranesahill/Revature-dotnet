const ErrorMessage = ({ message, onRetry }) => (
  <div className="error-container">
    <p className="error-icon">⚠</p>
    <p className="error-text">{message}</p>
    {onRetry && (
      <button className="btn-retry" onClick={onRetry}>Try Again</button>
    )}
  </div>
)

export default ErrorMessage
