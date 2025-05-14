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

//edit it
export const takeBook = async (idOrIsbn) => {
  const accessToken = localStorage.getItem("accessToken");

  const response = await axios.post(ApiEndpoints.Book.Take(idOrIsbn), {
    headers: {
      Authorization: `Bearer ${accessToken}`
    },
  });
  return response.data;
};

//edit it
export const deleteBook = async (id) => {
  const response = await axios.delete(ApiEndpoints.Book.Delete(id));
  return response.data;
};
