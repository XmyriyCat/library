// src/components/Navbar.js
import React, { useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import logout from "../utils/logout";
import refreshToken from "../utils/refreshToken";

const Navbar = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [username, setUsername] = useState("");

    useEffect(() => {
        const checkAuth = async () => {
            let token = localStorage.getItem("accessToken");

            if (token) {
                try {
                    let decoded = jwtDecode(token);
                    const isExpired = decoded.exp * 1000 < Date.now();

                    if (isExpired) {
                        await refreshToken();
                        token = localStorage.getItem("accessToken");
                        if (token) decoded = jwtDecode(token);
                    }

                    if (token && decoded.exp * 1000 > Date.now()) {
                        setIsAuthenticated(true);
                        setUsername(decoded.name);
                    } else {
                        setIsAuthenticated(false);
                        setUsername("");
                    }
                } catch (err) {
                    console.error("Token check failed", err);
                    setIsAuthenticated(false);
                    setUsername("");
                }
            } else {
                setIsAuthenticated(false);
                setUsername("");
            }
        };

        checkAuth();
    }, [location]);

    const handleLogout = () => {
        logout();
        setIsAuthenticated(false);
        setUsername("");
        navigate("/dashboard");
    };

    const hideOnAuthRoutes = ["/login", "/register"];
    if (hideOnAuthRoutes.includes(location.pathname)) {
        return null;
    }

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container-fluid">
                <Link className="navbar-brand text-light" to="/dashboard">
                    Library Dashboard
                </Link>
                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarNav"
                    aria-controls="navbarNav"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon" />
                </button>

                <div className="collapse navbar-collapse" id="navbarNav">
                    <ul className="navbar-nav ms-auto">
                        {!isAuthenticated ? (
                            <>
                                <li className="nav-item">
                                    <Link className="nav-link text-light" to="/login">
                                        Login
                                    </Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link text-light" to="/register">
                                        Registration
                                    </Link>
                                </li>
                            </>
                        ) : (
                            <>
                                <li className="nav-item d-flex align-items-center me-3">
                                    <span className="nav-link text-light disabled">
                                        {username}
                                    </span>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link text-light" to="/me">
                                        Me
                                    </Link>
                                </li>
                                <li className="nav-item">
                                    <button className="btn nav-link text-light" onClick={handleLogout}>
                                        Logout
                                    </button>
                                </li>
                            </>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
