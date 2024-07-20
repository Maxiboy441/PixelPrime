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
        const searchTerm = event.target.value;

        resultsWrapper.innerHTML = "";

        if (!searchTerm) {
            // Hide the dropdown if input is empty
            dropdown.classList.remove("is-active");
            dropdownMenu.style.visibility = "hidden";
            return;
        }

        const items = await fetchData(event.target.value);

        if (!items.length) {
            resultsWrapper.innerHTML = "";
            const notFoundMessage = document.createElement("div");
            notFoundMessage.classList.add("dropdown-item");
            notFoundMessage.textContent = "No results found.";
            resultsWrapper.appendChild(notFoundMessage);
            dropdown.classList.add("is-active");
            dropdownMenu.style.visibility = "visible";
            return;
        }

        resultsWrapper.innerHTML = ""; //clear search
        dropdown.classList.add("is-active");
        dropdownMenu.style.visibility = "visible";
        for (let item of items) {
            const option = document.createElement("a")

            option.href = `/movies/${item.imdbID}`;

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
        const response = await axios.get(`/Search?s=${searchTerm}`, {
            params: {
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
})
