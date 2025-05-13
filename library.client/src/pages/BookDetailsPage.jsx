import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { fetchBookById, getBookImage, updateBook, deleteBook, takeBook } from "../services/libraryService";

export default function BookDetails() {
  const params = useParams();
  const id = params.id;

  const [book, setBook] = useState(1);
  const [userRole, setUserRole] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [image, setImage] = useState(null);

  useEffect(() => {
    const fetchBook = async () => {
      try {
        const recievedBook = await fetchBookById(id);
        console.log(recievedBook);
        setBook(recievedBook);
      } catch (error) {
        console.error("Error fetching book:", error);
      }
    };

    const token = localStorage.getItem("accessToken");

    const fetchImage = async () => {
      try {
        const res = await getBookImage(id);
        if (!res.ok) throw new Error("Image fetch failed");
    
        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        setImage(url);
      } catch (err) {
        console.error("Image fetch error:", err);
      }
    };

    if (token) {
      const decoded = jwtDecode(token);
      // Assuming roles are in the 'roles' property
      const roles = decoded.role || [];
      if (roles.includes("admin")) {
        setUserRole("admin");
      } else if (roles.includes("manager")) {
        setUserRole("manager");
      } else {
        setUserRole("user"); // No roles or other roles
      }
    }
    fetchBook();
    fetchImage();
  }, [id]);

  const handleTakeBook = async (bookId) => {
    try {
      await takeBook(bookId);
    } catch (error) {
      console.error("Error taking book:", error);
    }
  };

  const handleUpdateBook = async (bookId) => {
    try {
      const response = await updateBook(bookId, book);
      setBook(response.data);
    } catch (error) {
      console.error("Error updating book:", error);
    } finally {
      setIsEditing(false);
    }
  };

  const handleDeleteBook = async (bookId) => {
    try {
      await deleteBook(bookId);
    } catch (error) {
      console.error("Error deleting book:", error);
    }
  };
  return (
    <div>
      {!isEditing && (
        <div>
          <h1>{book.title}</h1>
          <p>{book.isbn}</p>
          <p>{book.genre}</p>
          <p>{book.description}</p>
          <p>{book.author?.name}</p>
          {image ? <img src={image} alt={book.title} /> : <p>Loading image...</p>}
          <button onClick={() => handleTakeBook(book.id)}>Take Book</button>
          {userRole === "admin" && (
            <>
              <button onClick={() => setIsEditing(true)}>Edit Book</button>
              <button onClick={() => handleDeleteBook(book.id)}>
                Delete Book
              </button>
            </>
          )}
          {userRole === "manager" && (
            <button onClick={() => setIsEditing(true)}>Edit Book</button>
          )}
        </div>
      )}

      {isEditing && (
        <form>
          <input
            type="text"
            value={book.title}
            onChange={(e) => setBook({ ...book, title: e.target.value })}
          />
          <input
            type="text"
            value={book.isbn}
            onChange={(e) => setBook({ ...book, isbn: e.target.value })}
          />
          <input
            type="text"
            value={book.genre}
            onChange={(e) => setBook({ ...book, genre: e.target.value })}
          />
          <input
            type="text"
            value={book.description}
            onChange={(e) => setBook({ ...book, description: e.target.value })}
          />
          <input
            type="file"
            value={image}
            accept="image/*"
            onChange={(e) => setImage(e.target.files[0])}
          />
          <button onClick={() => handleUpdateBook(book.id)}>
            Save Changes
          </button>
        </form>
      )}
    </div>
  );
}
