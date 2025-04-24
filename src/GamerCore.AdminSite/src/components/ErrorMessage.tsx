function ErrorMessage({ message }: { message: string }) {
  return (
    <div className="alert alert-danger m-3" role="alert">
      <i className="bi bi-exclamation-triangle-fill me-2"></i>
      Error: {message}
    </div>
  );
}

export default ErrorMessage;