document.addEventListener('DOMContentLoaded', () => {
    // Base API URL
    const API_BASE_URL = 'https://localhost:7273/api/Account';

    // Function to decode JWT token
    function decodeJwt(token) {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
            return JSON.parse(jsonPayload);
        } catch (error) {
            console.error('Error decoding JWT:', error);
            return null;
        }
    }

    // ==============================
    // Handle Google Login Response on Dashboard Load
    // ==============================
    const currentUrl = new URL(window.location.href);
    if (currentUrl.pathname.includes('/sahla-frontend/User/html/home.html')) {
        const tokenParam = currentUrl.searchParams.get('token');
        if (tokenParam && tokenParam.split('.').length === 3) {
            localStorage.setItem('jwtToken', tokenParam);
            alert('Google login successful!');
            // Clean up the URL
            window.history.replaceState({}, document.title, '/sahla-frontend/User/html/home.html');
        }
    }

    // ==============================
    // Register Page Logic
    // ==============================
    const registerContainer = document.querySelector('.register-container');
    if (registerContainer) {
        const registerBtn = document.querySelector('.register-btn');
        const googleBtn = document.querySelector('.google-btn');
        const usernameInput = document.querySelector('#username');
        const emailInput = document.querySelector('#email');
        const passwordInput = document.querySelector('#password');
        const confirmPasswordInput = document.querySelector('#confirm-password');
        const addressInput = document.querySelector('#address');
        const phoneNumberInput = document.querySelector('#phone-number');
        const errorMessages = document.querySelectorAll('.error-message');

        // Clear error messages
        function clearErrors() {
            errorMessages.forEach(msg => (msg.style.display = 'none'));
        }

        // Show error message for a specific field
        function showError(input, message) {
            const errorMessage = input.parentElement.querySelector('.error-message');
            if (errorMessage) {
                errorMessage.textContent = message;
                errorMessage.style.display = 'block';
            }
        }

        // Validate register form
        function validateRegisterForm() {
            let isValid = true;
            clearErrors();

            if (!usernameInput.value.trim()) {
                showError(usernameInput, 'The Username field is required.');
                isValid = false;
            }

            if (!emailInput.value.trim()) {
                showError(emailInput, 'The Email field is required.');
                isValid = false;
            } else if (!/\S+@\S+\.\S+/.test(emailInput.value)) {
                showError(emailInput, 'Please enter a valid email address.');
                isValid = false;
            }

            if (!passwordInput.value) {
                showError(passwordInput, 'The Password field is required.');
                isValid = false;
            } else if (passwordInput.value.length < 6) {
                showError(passwordInput, 'Password must be at least 6 characters long.');
                isValid = false;
            }

            if (!confirmPasswordInput.value) {
                showError(confirmPasswordInput, 'Please confirm your password.');
                isValid = false;
            } else if (passwordInput.value !== confirmPasswordInput.value) {
                showError(confirmPasswordInput, 'Passwords do not match.');
                isValid = false;
            }

            if (!addressInput.value.trim()) {
                showError(addressInput, 'The Address field is required.');
                isValid = false;
            }

            if (!phoneNumberInput.value.trim()) {
                showError(phoneNumberInput, 'The Phone Number field is required.');
                isValid = false;
            } else if (!/^\+?\d{10,15}$/.test(phoneNumberInput.value.trim())) {
                showError(phoneNumberInput, 'Please enter a valid phone number (10-15 digits).');
                isValid = false;
            }

            return isValid;
        }

        // Handle register form submission
        registerBtn.addEventListener('click', async (e) => {
            e.preventDefault();

            if (!validateRegisterForm()) {
                return;
            }

            const registerData = {
                username: usernameInput.value.trim(),
                email: emailInput.value.trim(),
                password: passwordInput.value,
                confirmPassword: confirmPasswordInput.value.trim(),
                address: addressInput.value.trim(),
                phoneNumber: phoneNumberInput.value.trim()
            };

            try {
                const response = await fetch(`${API_BASE_URL}/register`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(registerData)
                });

                // Check if response is JSON
                const contentType = response.headers.get('Content-Type');
                let responseData = null;
                if (contentType && (contentType.includes('application/json') || contentType.includes('application/problem+json'))) {
                    responseData = await response.json();
                } else {
                    responseData = await response.text();
                    console.log('Non-JSON response:', responseData);
                }

                if (response.ok) {
                    alert('Registration successful!');
                    window.location.href = '/html/login.html';
                } else {
                    if (responseData && (contentType.includes('application/json') || contentType.includes('application/problem+json'))) {
                        if (responseData.errors) {
                            // Handle ASP.NET Core validation errors
                            if (responseData.errors.Address) {
                                showError(addressInput, responseData.errors.Address.join('; '));
                            }
                            if (responseData.errors.PhoneNumber) {
                                showError(phoneNumberInput, responseData.errors.PhoneNumber.join('; '));
                            }
                            if (responseData.errors.ConfirmPassword) {
                                showError(confirmPasswordInput, responseData.errors.ConfirmPassword.join('; '));
                            }
                            if (responseData.errors.Username || responseData.errors.UserName) {
                                showError(usernameInput, (responseData.errors.Username || responseData.errors.UserName).join('; '));
                            }
                            if (responseData.errors.Email) {
                                showError(emailInput, responseData.errors.Email.join('; '));
                            }
                            if (responseData.errors.Password) {
                                showError(passwordInput, responseData.errors.Password.join('; '));
                            }
                            // Handle Identity errors
                            if (responseData.errors.length && responseData.errors[0].code) {
                                responseData.errors.forEach(error => {
                                    if (error.code.includes('DuplicateUserName')) {
                                        showError(usernameInput, 'Username is already taken.');
                                    } else if (error.code.includes('DuplicateEmail')) {
                                        showError(emailInput, 'Email is already registered.');
                                    } else if (error.code.includes('Password')) {
                                        showError(passwordInput, error.description);
                                    } else {
                                        alert('Registration failed: ' + error.description);
                                    }
                                });
                            }
                        } else {
                            alert('Registration failed: ' + JSON.stringify(responseData));
                        }
                    } else {
                        alert('Registration failed: Server returned non-JSON response: ' + responseData);
                    }
                }
            } catch (error) {
                console.error('Error during registration:', error);
                alert('An error occurred. Please try again later.');
            }
        });

        // Handle Google sign-up
        googleBtn.addEventListener('click', () => {
            window.location.href = `${API_BASE_URL}/google-login?returnUrl=/sahla-frontend/User/html/home.html`;
        });
    }

    // ==============================
    // Login Page Logic
    // ==============================
    const loginForm = document.querySelector('#loginForm');
    if (loginForm) {
        const loginBtn = document.querySelector('.login-btn');
        const registerBtn = document.querySelector('.register-btn');
        const googleBtn = document.querySelector('#googleLogin');
        const emailInput = document.querySelector('#email');
        const passwordInput = document.querySelector('#password');
        const rememberMeInput = document.querySelector('#rememberMe');
        const errorMessage = document.querySelector('#loginError');

        // Clear error message
        function clearError() {
            errorMessage.textContent = '';
            errorMessage.style.display = 'none';
        }

        // Show error message
        function showError(message) {
            errorMessage.textContent = message;
            errorMessage.style.display = 'block';
        }

        // Validate login form
        function validateLoginForm() {
            let isValid = true;
            clearError();

            if (!emailInput.value.trim()) {
                showError('The Email field is required.');
                isValid = false;
            } else if (!/\S+@\S+\.\S+/.test(emailInput.value)) {
                showError('Please enter a valid email address.');
                isValid = false;
            }

            if (!passwordInput.value) {
                showError('The Password field is required.');
                isValid = false;
            }

            return isValid;
        }

        // Handle login form submission
        loginBtn.addEventListener('click', async (e) => {
            e.preventDefault();

            if (!validateLoginForm()) {
                return;
            }

            const loginData = {
                email: emailInput.value.trim(),
                password: passwordInput.value,
                rememberMe: rememberMeInput.checked
            };

            try {
                const response = await fetch(`${API_BASE_URL}/Login`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(loginData)
                });

                if (response.ok) {
                    const data = await response.json();
                    localStorage.removeItem('jwtToken');
localStorage.setItem('jwtToken', data.token);
console.log(data); // شوف شكله
                    alert('Login successful!');
                   window.location.href = '/sahla-frontend/User/html/home.html';
                } else if (response.status === 400) {
    const errorData = await response.json();
    if (errorData.errors && errorData.errors.Error) {
        const emailGroup = emailInput.parentElement;
        const emailError = emailGroup.querySelector('.error-message');
        if (emailError) {
            emailError.textContent = errorData.errors.Error;
            emailError.style.display = 'block';
        }
    } else {
        showError('Invalid email or password.');
    }
                } else if (response.status === 404) {
                    showError('User not found.');
                } else {
                    showError('An error occurred. Please try again.');
                }
            } catch (error) {
                console.error('Error during login:', error);
                showError('An error occurred. Please try again later.');
            }
        });

        // Handle register button click
        registerBtn.addEventListener('click', (e) => {
            e.preventDefault();
            window.location.href = '/sahla-frontend/User/html/register.html';
        });

        // Handle Google login
        googleBtn.addEventListener('click', () => {
            window.location.href = `${API_BASE_URL}/google-login?returnUrl=/sahla-frontend/User/html/home.html`;
        });
    }

    // ==============================
    // Dashboard Page Logic
    // ==============================
    const dashboardContainer = document.querySelector('.dashboard-container');
    if (dashboardContainer) {
        const welcomeMessage = document.querySelector('#welcome-message');
        const logoutBtn = document.querySelector('.logout-btn');
        const errorMessage = document.querySelector('#logout-error');

        // Display welcome message
        const token = localStorage.getItem('jwtToken');
        if (token) {
            const decoded = decodeJwt(token);
            if (decoded && decoded.name) {
                welcomeMessage.textContent = `Welcome, ${decoded.name}!`;
            } else {
                welcomeMessage.textContent = 'Welcome!';
            }
        } else {
            // Redirect to login if no token
            window.location.href = '/sahla-frontend/User/html/login.html';

        }

        // Clear error message
        function clearError() {
            errorMessage.textContent = '';
            errorMessage.style.display = 'none';
        }

        // Show error message
        function showError(message) {
            errorMessage.textContent = message;
            errorMessage.style.display = 'block';
        }

        // Handle logout
        logoutBtn.addEventListener('click', async () => {
            try {
                const response = await fetch(`${API_BASE_URL}/logout`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    localStorage.removeItem('jwtToken');
                    alert('Logout successful!');
                    window.location.href = '/html/login.html';
                } else {
                    showError('Logout failed. Please try again.');
                }
            } catch (error) {
                console.error('Error during logout:', error);
                showError('An error occurred. Please try again later.');
            }
        });
    }
});