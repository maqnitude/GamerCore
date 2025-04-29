import { Category } from "../../../types";

interface CategoriesTableProps {
  categories: Category[]
}

function CategoriesTable({ categories }: CategoriesTableProps) {
  return (
    <div className="table-responsive">
      <div className="table table-striped table-hover">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Description</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr>
              <td>{category.categoryId}</td>
              <td>{category.name}</td>
              <td>{category.description}</td>
              <td>
                <div className="btn-group btn-group-sm float-end">
                  <button className="btn btn-outline-primary d-flex flex-row align-items-center">
                    <i className="bi bi-pencil me-2"></i>
                    Edit
                  </button>
                  <button className="btn btn-outline-danger d-flex flex-row align-items-center">
                    <i className="bi bi-trash me-2"></i>
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </div>
    </div>
  );
}

export default CategoriesTable;