const API_BASE_URL = process.env.REACT_APP_API_BASE_URL; // Base api URL is here

export const ApiEndpoints = {
  Author: {
    Create: `${API_BASE_URL}/authors`,
    Get: (id) => `${API_BASE_URL}/authors/${id}`,
    GetAll: (page, pagesize) => `${API_BASE_URL}/authors?pagesize=${pagesize}&page=${page}`,
    Update: (id) => `${API_BASE_URL}/authors/${id}`,
    Delete: (id) => `${API_BASE_URL}/authors/${id}`,
    GetBooks: (id) => `${API_BASE_URL}/authors/${id}/books`,
  },
  Book: {
    Create: `${API_BASE_URL}/books`,
    Get: (idOrIsbn) => `${API_BASE_URL}/books/${idOrIsbn}`,
    GetAll: (page, pagesize, title, genre, author) => `${API_BASE_URL}/books?pagesize=${pagesize}&page=${page}&title=${title}&genre=${genre}&author=${author}`,
    Update: (id) => `${API_BASE_URL}/books/${id}`,
    Delete: (id) => `${API_BASE_URL}/books/${id}`,
    GetImage: (idOrIsbn) => `${API_BASE_URL}/books/${idOrIsbn}/image`,
  },
  Auth: {
    Register: `${API_BASE_URL}/auth/register`,
    Login: `${API_BASE_URL}/auth/login`,
    Refresh: `${API_BASE_URL}/auth/refresh`,
  },
  UserBook: {
    Create: `${API_BASE_URL}/me/books`,
    GetAll: `${API_BASE_URL}/me/books`,
    Delete: (bookId) => `${API_BASE_URL}/me/books/${bookId}`,
  },
};

export default ApiEndpoints;