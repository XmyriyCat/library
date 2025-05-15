import axios from "../api/axiosInstance";
import ApiEndpoints from "../api/ApiEndpoints";

export const fetchBooks = async ({ page = 1, pageSize = 6, title = "", genre = "", author = "" }) => {
  const url = ApiEndpoints.Book.GetAll(page, pageSize, title, genre, author);
  const response = await axios.get(url);
  return response.data;
};

export const fetchAuthors = async ({ page = 1, pageSize = 6 }) => {
  const response = await axios.get(ApiEndpoints.Author.GetAll(page, pageSize));
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

export const updateBook = async (id, formData) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.put(ApiEndpoints.Book.Update(id), formData, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "multipart/form-data",
    },
  });
  return response.data;
};

export const createBook = async (formData) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.post(ApiEndpoints.Book.Create, formData, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "multipart/form-data",
    },
  });
  return response.data;
};

export const takeBook = async (id) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.post(ApiEndpoints.UserBook.Create,
    { BookId: id },
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json"
      },
    }
  );

  return response.data;
};

export const deleteBook = async (id) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.delete(ApiEndpoints.Book.Delete(id),
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json"
      },
    });

  return response.data;
};

export const fetchAuthorById = async (id) => {
  const response = await axios.get(ApiEndpoints.Author.Get(id));
  return response.data;
};

export const updateAuthor = async (id, data) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.put(ApiEndpoints.Author.Update(id), data, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "application/json"
    },
  });

  return response.data;
};

export const createAuthor = async (data) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.post(ApiEndpoints.Author.Create, data, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "application/json"
    },
  });

  return response.data;
};

export const deleteAuthor = async (id) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.delete(ApiEndpoints.Author.Delete(id),
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json"
      },
    });

  return response.data;
};