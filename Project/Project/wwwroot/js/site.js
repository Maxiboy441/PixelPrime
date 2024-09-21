// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Profile side card
document.addEventListener('DOMContentLoaded', (event) => {
    const mySpaceMovies = document.querySelectorAll(".movie-card");

    mySpaceMovies.forEach(movie => {
        movie.addEventListener("click", async (e) => {
            const movieId = movie.getAttribute("data-imdb-id");
            const movieDetailsId = movie.getAttribute('data-movie-id');
            const detailsContainer = document.getElementById(`movie-details-${movieDetailsId}`);

            try {
                const response = await axios.get(`/SearchById?id=${movieId}`);
                const { Genre, Plot, imdbID } = response.data;

                displayMovieDetails(detailsContainer, response.data);
            } catch (error) {
                console.error(error);
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
                        const movieCard = form.closest('.movie-card');
                        const movieId = movieCard.getAttribute('data-movie-id');
                        const sideCard = document.querySelector(`.side-card[data-movie-id="${movieId}"]`);

                        displayFlashMessage('alert-success', data.message);
                        movieCard.remove();

                        if (sideCard && !sideCard.classList.contains('d-none')) {
                            sideCard.classList.add('hide');
                            sideCard.addEventListener('animationend', () => sideCard.classList.add('d-none'), { once: true });
                        }
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
        flashMessageContainer.className = `alert ${alertType} alert-dismissible fade show custom-alert`;
        flashMessageContainer.role = 'alert';
        flashMessageContainer.innerHTML = `${message}<button type="button" class="btn btn-close" data-dismiss="alert" aria-label="Close" />`;

        const container = document.querySelector('.content-wrapper');
        container.appendChild(flashMessageContainer);

        setTimeout(() => {
            flashMessageContainer.classList.remove('show');
            flashMessageContainer.classList.add('fade');
            setTimeout(() => flashMessageContainer.remove(), 150);
        }, 3500);
    }


    // General flash messages
    const flashContainer = document.querySelector(".flash-container");
    const flashMessage = flashContainer.querySelector('.alert');

    if (flashContainer) {
        setTimeout(() => {
            flashContainer.classList.remove('show');
            flashContainer.classList.add('fade');
        }, 3500);
    }


    const dropdownToggle = document.getElementById('navbarDropMenu');
    const dropdownMenu = dropdownToggle.nextElementSibling;

    dropdownToggle.addEventListener('click', function(event) {
        event.preventDefault();
        dropdownMenu.classList.toggle('show');
    });

    document.addEventListener('click', function(event) {
        const isClickInside = dropdownToggle.contains(event.target) || dropdownMenu.contains(event.target);

        if (!isClickInside) {
            dropdownMenu.classList.remove('show');
        }
    });

    document.querySelectorAll('.delete-confirmation-btn').forEach(button => {
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
                        const reviewContainer = form.closest('.review-container');
                        const deleteModalElement = document.getElementById('deleteConfirmationModal');
                        const deleteModal = new bootstrap.Modal(deleteModalElement);

                        displayFlashMessage('alert-success', data.message);
                        reviewContainer.remove();
                        deleteModal.hide();
                        const backdrop = document.querySelector('.modal-backdrop');
                        if (backdrop) {
                            backdrop.classList.remove('show');
                            backdrop.remove();
                        }

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
});

// Movies Slider
const swiperEl = document.querySelector('swiper-container')

if (swiperEl) {
    Object.assign(swiperEl, {
        breakpoints: {
            345: {
                slidesPerView: 1,
                spaceBetween: 150,
                centeredSlides: true,
            },
            577: {
                slidesPerView: 3,
                spaceBetween: 150,
                centeredSlides: false,
            },
            600: {
                slidesPerView: 2,
                spaceBetween: 150,
                centeredSlides: false,
            },
            768: {
                slidesPerView: 3,
                spaceBetween: 250,
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
}

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


// Edit Review Form
function checkEditForm() {
    const title = document.getElementById('editReviewTitle').value;
    const review = document.getElementById('editReviewText').value;
    const submitBtn = document.getElementById('submitBtn');

    var isTitleValid = title.trim() !== "";
    var isReviewValid = review.trim() !== "";

    if (isTitleValid) {
        document.getElementById('editReviewTitle').classList.remove('invalid-input');
    } else {
        document.getElementById('editReviewTitle').classList.add('invalid-input');
    }

    if (isReviewValid) {
        document.getElementById('editReviewText').classList.remove('invalid-input');
    } else {
        document.getElementById('editReviewText').classList.add('invalid-input');
    }

    if (isTitleValid && isReviewValid) {
        submitBtn.removeAttribute('disabled');
    } else {
        submitBtn.setAttribute('disabled', 'disabled');
    }
}

function clearEditForm() {
    document.getElementById('editReviewTitle').value = '';
    document.getElementById('editReviewText').value = '';
    document.getElementById('submitBtn').setAttribute('disabled', 'disabled');
    document.getElementById('editReviewTitle').classList.remove('invalid-input');
    document.getElementById('editReviewText').classList.remove('invalid-input');
}

document.querySelector('#editReviewModal .close').addEventListener('click', clearEditForm);
document.getElementById('editReviewModal').addEventListener('hidden.bs.modal', clearEditForm);
document.getElementById('submitBtn').addEventListener('click', function (event) {
    event.preventDefault();
    clearEditForm();
    $('#editReviewModal').modal('hide');
});