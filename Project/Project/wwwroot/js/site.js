﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Profile side card
document.addEventListener('DOMContentLoaded', (event) => {
    const mySpaceMovies = document.querySelectorAll(".movie-card");

    mySpaceMovies.forEach(movie => {
        movie.addEventListener("click", async (e) => {
            const movieId = movie.getAttribute("data-imdb-id");
            const detailsContainer = document.getElementById(`movie-details-${movieId}`);

            // Check if the movie details are in session storage
            const storedMovie = sessionStorage.getItem(`movie_${movieId}`);
            if (storedMovie) {
                // If found in session storage, use the stored data
                const movieData = JSON.parse(storedMovie);
                displayMovieDetails(detailsContainer, movieData);
            } else {
                // If not found, fetch from API
                try {
                    const response = await axios.get(`/SearchById?id=${movieId}`);
                    const { Genre, Plot, imdbID } = response.data;

                    // Store the fetched movie details in session storage
                    sessionStorage.setItem(`movie_${movieId}`, JSON.stringify(response.data));

                    displayMovieDetails(detailsContainer, response.data);
                } catch (error) {
                    console.error(error);
                }
            }
        });
    });

    function displayMovieDetails(container, movieData) {
        container.innerHTML = `
            <h5 class="card-title mt-3">${movieData.Title}</h5>
            <p class="card-title">${movieData.Genre}</p>
            <hr />
            <p class="card-title">${movieData.Plot}</p>
        `;
    }

    const movieCards = document.querySelectorAll('.movie-card');
    const sideCards = document.querySelectorAll('.side-card');
    const sideCardContainer = document.getElementById('side-card-container');

    movieCards.forEach(card => {
        card.addEventListener('click', (event) => {
            if (event.target.classList.contains('close-btn')) {
                // Prevent the side container from opening if the close button is clicked
                event.stopPropagation();
                return;
            }

            const movieId = card.getAttribute('data-movie-id');
            const sideCard = document.querySelector(`.side-card[data-movie-id="${movieId}"]`);

            sideCards.forEach(sc => {
                if (!sc.classList.contains('d-none')) {
                    sc.classList.add('hide');
                    sc.addEventListener('animationend', () => sc.classList.add('d-none'), { once: true });
                }
            });

            setTimeout(() => {
                if (sideCard) {
                    sideCard.classList.remove('d-none', 'hide');
                    sideCard.classList.add('show');
                }
            }, 700);

            document.getElementById('top').scrollIntoView({ behavior: 'smooth' });

        });
    });

    sideCards.forEach(card => {
        const closeButton = card.querySelector('.close-side-container');
        closeButton.addEventListener('click', (e) => {
            e.stopPropagation();
            card.classList.add('hide');
            card.addEventListener('animationend', () => card.classList.add('d-none'), { once: true });
        });
    });


    // Handle delete Favorites or Watchlist
    document.querySelectorAll('.delete-profile-button').forEach(button => {
        button.addEventListener('click', function (event) {
            event.preventDefault();

            const form = this.closest('form');
            const url = form.action;
            const formData = new FormData(form);

            fetch(url, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        displayFlashMessage('alert-success', data.message);
                        form.closest('.movie-card').remove();
                    } else {
                        if (data.redirectToLogin) {
                            window.location.href = '/Auth/Login';
                        } else {
                            displayFlashMessage('alert-danger', data.message);
                        }
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    displayFlashMessage('alert-danger', 'Error deleting movie.');
                });
        });
    });

    function displayFlashMessage(alertType, message) {
        const flashMessageContainer = document.createElement('div');
        flashMessageContainer.className = `alert ${alertType} alert-dismissible fade show profile-alert`;
        flashMessageContainer.role = 'alert';
        flashMessageContainer.innerHTML = `${message}<button type="button" class="btn btn-close" data-dismiss="alert" aria-label="Close" />`;

        const container = document.querySelector('.profile-container');
        container.appendChild(flashMessageContainer);

        setTimeout(() => {
            flashMessageContainer.classList.remove('show');
            flashMessageContainer.classList.add('fade');
            setTimeout(() => flashMessageContainer.remove(), 150);
        }, 5000);
    }
});

// Movies Slider
const swiperEl = document.querySelector('swiper-container')
Object.assign(swiperEl, {
    breakpoints: {
        345: {
            slidesPerView: 2,
            spaceBetween: 15,
            centeredSlides: true,
        },
        640: {
            slidesPerView: 2,
            spaceBetween: 10,
            centeredSlides: false,
        },
        768: {
            slidesPerView: 2,
            spaceBetween: 10,
            centeredSlides: false,
        },
        769: {
            slidesPerView: 2,
            spaceBetween: 10,
            centeredSlides: false,
        },
        992: {
            slidesPerView: 3,
            spaceBetween: 10,
            centeredSlides: false
        },
        1024: {
            slidesPerView: 3,
            spaceBetween: 10,
            centeredSlides: false
        },
        1245: {
            slidesPerView: 3,
            spaceBetween: 10,
            centeredSlides: false
        },
        1399: {
            slidesPerView: 4,
            spaceBetween: 10,
            centeredSlides: false
        },
    },
});
swiperEl.initialize();


// Movie Rating Star
const stars = document.querySelectorAll('.star');
const form = document.getElementById('ratingForm');

function setRating(value) {
    document.getElementById('ratingValue').value = value;

    stars.forEach(s => s.classList.remove('selected'));
    for (let i = 0; i < value; i++) {
        stars[i].classList.add('selected');
    }

    form.submit();
}

stars.forEach(star => {
    star.addEventListener('mouseover', () => {
        const value = star.getAttribute('data-value');
        stars.forEach((s, index) => {
            s.classList.toggle('hovered', index < value);
        });
    });

    star.addEventListener('mouseout', () => {
        stars.forEach(s => s.classList.remove('hovered'));
        const selectedValue = document.getElementById('ratingValue').value;
        stars.forEach((s, index) => {
            s.classList.toggle('selected', index < selectedValue);
        });
    });
});


// Pop up review form
function checkForm() {
    const title = document.getElementById('reviewTitle').value;
    const review = document.getElementById('reviewText').value;
    const submitBtn = document.getElementById('submitBtn');

    var isTitleValid = title.trim() !== "";
    var isReviewValid = review.trim() !== "";

    if (isTitleValid) {
        document.getElementById('reviewTitle').classList.remove('invalid-input');
    } else {
        document.getElementById('reviewTitle').classList.add('invalid-input');
    }

    if (isReviewValid) {
        document.getElementById('reviewText').classList.remove('invalid-input');
    } else {
        document.getElementById('reviewText').classList.add('invalid-input');
    }

    if (isTitleValid && isReviewValid) {
        submitBtn.removeAttribute('disabled');
    } else {
        submitBtn.setAttribute('disabled', 'disabled');
    }
}

function clearForm() {
    document.getElementById('reviewTitle').value = '';
    document.getElementById('reviewText').value = '';
    document.getElementById('submitBtn').setAttribute('disabled', 'disabled');
    document.getElementById('reviewTitle').classList.remove('invalid-input');
    document.getElementById('reviewText').classList.remove('invalid-input');
}

document.querySelector('#myModal .close').addEventListener('click', clearForm);
document.getElementById('myModal').addEventListener('hidden.bs.modal', clearForm);
document.getElementById('submitBtn').addEventListener('click', function (event) {
    event.preventDefault();
    clearForm();
    $('#myModal').modal('hide');
});