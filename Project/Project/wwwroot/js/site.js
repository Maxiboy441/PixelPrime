// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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
            slidesPerView: 4,
            spaceBetween: 20,
            centeredSlides: false,
        },
        1024: {
            slidesPerView: 4,
            spaceBetween: 20,
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

