const debounce = (func, delay = 1000) => { //default delay
    let timeoutId;
    return (...args) => {
        if (timeoutId) {
            clearTimeout(timeoutId)
        }
        timeoutId = setTimeout(() => {
            func.apply(null, args);
        }, delay)
    }
};

const createAutoComplete = ({
    root,
    renderOption,
    onOptionSelect,
    inputValue,
    fetchData
}) => {
    root.innerHTML = `
        <div class="dropdown w-100">
            <input class="input me-2 form-control" type="text" placeholder="Search for a movie" id="searchInput" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">

            <div class="dropdown-menu" aria-labelledby="searchInput">
                <div class="results"></div>
            </div>
        </div> 
    `

    const input = root.querySelector("input");
    const dropdown = root.querySelector(".dropdown");
    const resultsWrapper = root.querySelector(".results");
    const dropdownMenu = root.querySelector(".dropdown-menu");

    const onInput = async event => {
        const items = await fetchData(event.target.value);

        if (!items.length) {
            dropdownMenu.style.visibility = "hidden";
            dropdown.classList.remove("is-active");
            resultsWrapper.innerHTML = "";
            return;
        }

        resultsWrapper.innerHTML = ""; //clear search
        dropdown.classList.add("is-active");
        dropdownMenu.style.visibility = "visible";
        for (let item of items) {
            const option = document.createElement("a")

            option.classList.add("dropdown-item")
            option.innerHTML = renderOption(item);
            option.addEventListener("click", () => {
                dropdown.classList.remove("is-active")
                input.value = inputValue(item);
                onOptionSelect(item)
            })


            resultsWrapper.appendChild(option)
        }
    };


    input.addEventListener("input", debounce(onInput, 800)); //here it can be added how much delay


    document.addEventListener("click", (event) => {
        if (!root.contains(event.target)) {
            dropdown.classList.remove("is-active")
        }
    })
};

const autoCompleteConfig = {
    renderOption(movie) {
        const imgSrc = movie.Poster === "N/A" ? "" : movie.Poster;
        return `
        <img src= "${imgSrc}" />
        ${movie.Title} (${movie.Year})
    `;
    },
    inputValue(movie) {
        return movie.Title;
    },
    async fetchData(searchTerm) {
        const response = await axios.get("https://www.omdbapi.com/", {
            params: {
                apikey: '29f872c7',
                s: searchTerm,
            }
        });

        if (response.data.Error) {
            return [];
        }

        return response.data.Search
    }
}


createAutoComplete({
    ...autoCompleteConfig,
    root: document.querySelector(".search"),
    //onOptionSelect(movie) {
    //    document.querySelector(".tutorial").classList.add("is-hidden")
    //    onMovieSelect(movie, document.querySelector("#left-summary"), "left");
    //},
})
