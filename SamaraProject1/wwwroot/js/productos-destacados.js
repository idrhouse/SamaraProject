/**
 * Carrusel de Productos Destacados - Sin Animaciones
 * Versión completamente optimizada para rendimiento
 */
document.addEventListener("DOMContentLoaded", () => {
    // Verificar si el carrusel existe en la página
    const carousel = document.getElementById("featured-carousel")
    if (!carousel) return

    console.log("Inicializando carrusel sin animaciones")

    // Elementos del DOM
    const slidesContainer = document.getElementById("featured-slides")
    const prevButton = document.getElementById("prevFeatured")
    const nextButton = document.getElementById("nextFeatured")
    const indicatorsContainer = document.getElementById("carousel-indicators")
    const productNameElement = document.getElementById("current-product-name")

    // Variables de estado
    let currentIndex = 0
    const slides = []
    let products = []
    let autoplayTimer = null

    // Cargar productos destacados
    fetch("/api/productos/destacados")
        .then((response) => response.json())
        .then((data) => {
            products = data
            if (products && products.length > 0) {
                initializeCarousel()
            } else {
                slidesContainer.innerHTML =
                    '<div class="flex items-center justify-center h-full"><p class="text-gray-500">No hay productos destacados disponibles</p></div>'
            }
        })
        .catch((error) => {
            console.error("Error al cargar productos destacados:", error)
            slidesContainer.innerHTML =
                '<div class="flex items-center justify-center h-full"><p class="text-gray-500">Error al cargar productos</p></div>'
        })

    // Inicializar el carrusel
    function initializeCarousel() {
        // Limpiar contenedores
        slidesContainer.innerHTML = ""
        indicatorsContainer.innerHTML = ""

        // Crear slides
        products.forEach((product, index) => {
            // Crear slide
            const slide = document.createElement("div")
            slide.className = "absolute inset-0 w-full h-full"
            slide.style.display = index === 0 ? "block" : "none"

            // Contenido del slide
            slide.innerHTML = `
                <div class="h-full">
                    <a href="/Producto/Detalles/${product.idProducto}" class="block h-full">
                        <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                        <img 
                            src="${product.imagenUrl}" 
                            alt="${product.nombre}" 
                            class="w-full h-full object-cover"
                            loading="${index < 2 ? "eager" : "lazy"}"
                        >
                        <div class="absolute bottom-0 left-0 right-0 p-4 sm:p-6 text-white">
                            <h3 class="text-lg sm:text-xl font-medium">${product.nombre}</h3>
                            <p class="text-sm sm:text-base text-white/80 mt-1">${product.precio}</p>
                        </div>
                    </a>
                </div>
            `

            slidesContainer.appendChild(slide)
            slides.push(slide)

            // Crear indicador
            const indicator = document.createElement("button")
            indicator.className = `w-2.5 h-2.5 rounded-full mx-1 ${index === 0 ? "bg-orange-500" : "bg-gray-300"}`
            indicator.setAttribute("aria-label", `Ver producto ${product.nombre}`)
            indicator.onclick = () => {
                goToSlide(index)
                resetAutoplay()
            }

            indicatorsContainer.appendChild(indicator)
        })

        // Mostrar nombre del producto inicial
        if (productNameElement && products.length > 0) {
            productNameElement.textContent = products[0].nombre
        }

        // Configurar navegación
        if (prevButton) {
            prevButton.onclick = () => {
                goToPrev()
                resetAutoplay()
            }
        }

        if (nextButton) {
            nextButton.onclick = () => {
                goToNext()
                resetAutoplay()
            }
        }

        // Iniciar autoplay
        startAutoplay()

        // Pausar autoplay cuando el mouse está sobre el carrusel
        carousel.addEventListener("mouseenter", stopAutoplay)
        carousel.addEventListener("mouseleave", startAutoplay)

        // Optimización: pausar cuando no es visible
        setupVisibilityObserver()
    }

    // Ir a un slide específico
    function goToSlide(index) {
        if (index < 0 || index >= slides.length || index === currentIndex) return

        // Ocultar slide actual
        slides[currentIndex].style.display = "none"

        // Actualizar indicadores
        const indicators = indicatorsContainer.querySelectorAll("button")
        indicators[currentIndex].classList.remove("bg-orange-500")
        indicators[currentIndex].classList.add("bg-gray-300")

        // Actualizar índice
        currentIndex = index

        // Mostrar nuevo slide
        slides[currentIndex].style.display = "block"

        // Actualizar indicadores
        indicators[currentIndex].classList.remove("bg-gray-300")
        indicators[currentIndex].classList.add("bg-orange-500")

        // Actualizar nombre del producto
        if (productNameElement && products[currentIndex]) {
            productNameElement.textContent = products[currentIndex].nombre
        }
    }

    // Ir al slide anterior
    function goToPrev() {
        const newIndex = currentIndex === 0 ? slides.length - 1 : currentIndex - 1
        goToSlide(newIndex)
    }

    // Ir al siguiente slide
    function goToNext() {
        const newIndex = currentIndex === slides.length - 1 ? 0 : currentIndex + 1
        goToSlide(newIndex)
    }

    // Iniciar autoplay
    function startAutoplay() {
        stopAutoplay()
        autoplayTimer = setTimeout(() => {
            goToNext()
            startAutoplay()
        }, 5000)
    }

    // Detener autoplay
    function stopAutoplay() {
        if (autoplayTimer) {
            clearTimeout(autoplayTimer)
            autoplayTimer = null
        }
    }

    // Reiniciar autoplay
    function resetAutoplay() {
        stopAutoplay()
        startAutoplay()
    }

    // Configurar observador de visibilidad
    function setupVisibilityObserver() {
        if ("IntersectionObserver" in window) {
            const observer = new IntersectionObserver(
                (entries) => {
                    if (entries[0].isIntersecting) {
                        startAutoplay()
                    } else {
                        stopAutoplay()
                    }
                },
                { threshold: 0.1 },
            )

            observer.observe(carousel)
        }
    }

    // Limpiar al desmontar
    window.addEventListener("beforeunload", () => {
        stopAutoplay()
    })
})
