interface PaginationProps {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  onPageChange: (page: number) => void;
}

function Pagination({ currentPage, pageSize, totalItems, onPageChange }: PaginationProps) {
  const totalPages = Math.ceil(totalItems / pageSize);

  if (totalPages <= 1) {
    return null;
  }

  return (
    <>
      <nav aria-label="Pagination">
        <ul className="pagination justify-content-center">
          <li className={`page-item ${currentPage === 1 ? "disabled" : ""}`}>
            <button
              className="page-link"
              onClick={() => onPageChange(currentPage - 1)}
              disabled={currentPage === 1}
            >
              <i className="bi bi-chevron-left"></i>
            </button>
          </li>

          {Array.from({ length: totalPages }, (_, i) => i + 1)
            .map(page => (
              <li key={page} className={`page-item ${page === currentPage ? "active" : ""}`}>
                <button
                  className="page-link"
                  onClick={() => onPageChange(page)}
                >
                  {page}
                </button>
              </li>
            ))}

          <li className={`page-item ${currentPage >= totalPages ? "disabled" : ""}`}>
            <button
              className="page-link"
              onClick={() => onPageChange(currentPage + 1)}
              disabled={currentPage >= totalPages}
            >
              <i className="bi bi-chevron-right"></i>
            </button>
          </li>
        </ul>
      </nav>

      <div className="text-center text-muted small">
        Page {currentPage} of {totalPages}
      </div>
    </>
  );
}

export default Pagination;