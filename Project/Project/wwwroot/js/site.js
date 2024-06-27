// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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