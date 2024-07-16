﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Profile side card
document.addEventListener('DOMContentLoaded', (event) => {
    const mySpaceMovies = document.querySelectorAll(".movie-card");

    mySpaceMovies.forEach(movie => {
        movie.addEventListener("click", async (e) => {
            const movieId = movie.getAttribute("data-imdb-id");

            console.log(movieId)
            try {
                const response = await axios.get(`/SearchById?id=${movieId}`);
                console.log(response.data);
                const detailsContainer = document.getElementById(`movie-details-${movieId}`);
                const { Genre, Plot, imdbID } = response.data;

                detailsContainer.innerHTML = `
                        <p class="card-title mt-3"><a href="#">${Genre}</a></p>
                        <hr />
                        <p class="card-title"><a href="#">${Plot}</a></p>
                    `;
            } catch (error) {
                console.error(error);
            }
        })
    });

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

            if (sideCard) {
                sideCard.classList.remove('d-none', 'hide');
                sideCard.classList.add('show');
            }
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