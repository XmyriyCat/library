import axios from '../api/axiosInstance';
import ApiEndpoints from '../api/ApiEndpoints';

export const fetchBooks = async () => {
  const response = await axios.get(ApiEndpoints.Book.GetAll);
  return response.data;
};

export const fetchAuthors = async () => {
  const response = await axios.get(ApiEndpoints.Author.GetAll);
  return response.data;
};