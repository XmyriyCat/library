import axios from "../api/axiosInstance";
import ApiEndpoints from "../api/ApiEndpoints";

export const fetchBooks = async () => {
  const response = await axios.get(ApiEndpoints.Book.GetAll);
  return response.data;
};

export const fetchAuthors = async () => {
  const response = await axios.get(ApiEndpoints.Author.GetAll);
  return response.data;
};

export const fetchBookById = async (id) => {
  const response = await axios.get(ApiEndpoints.Book.Get(id));
  return response.data;
};

export const getBookImage = async (idOrIsbn) => {
  const response = await fetch(ApiEndpoints.Book.GetImage(idOrIsbn));
  return response;
};

// edit it
export const takeBook = async (idOrIsbn) => {
  const response = await axios.get(ApiEndpoints.Book.GetImage(idOrIsbn));
  return response.data;
};

// edit it
export const updateBook = async (id) => {
  const response = await axios.get(ApiEndpoints.Book.GetImage(id));
  return response.data;
};

// edit it
export const deleteBook = async (id) => {
  const response = await axios.get(ApiEndpoints.Book.GetImage(id));
  return response.data;
};