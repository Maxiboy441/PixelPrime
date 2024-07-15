﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Profile side card
document.addEventListener('DOMContentLoaded', (event) => {
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

