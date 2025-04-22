/**
 * YouTube Facade - Carga diferida para videos de YouTube
 * Mejora el rendimiento del sitio cargando el reproductor de YouTube solo cuando el usuario hace clic
 */
class YouTubeFacade {
  constructor(options = {}) {
    this.options = {
      containerClass: "youtube-facade",
      playButtonClass: "youtube-facade-play",
      loadingClass: "youtube-facade-loading",
      playerClass: "youtube-facade-player",
      ...options,
    }

    this.init()
  }

  init() {
    // Inicializar todos los contenedores de video al cargar la página
    if (document.readyState === "loading") {
      document.addEventListener("DOMContentLoaded", () => {
        this.initContainers()
      })
    } else {
      // Si el DOM ya está cargado, inicializar inmediatamente
      this.initContainers()
    }
  }

  initContainers() {
    const containers = document.querySelectorAll(`.${this.options.containerClass}`)
    containers.forEach((container) => this.setupContainer(container))
    console.log(`Inicializados ${containers.length} contenedores de video`)
  }

  setupContainer(container) {
    const videoId = container.dataset.videoId
    if (!videoId) {
      console.error("Contenedor sin ID de video:", container)
      return
    }

    console.log(`Configurando contenedor para video: ${videoId}`)

    // Crear la fachada con la miniatura y el botón de reproducción
    this.createFacade(container, videoId)

    // Agregar evento de clic para cargar el video
    container.addEventListener("click", (e) => {
      e.preventDefault()
      this.loadYouTubePlayer(container, videoId)
    })
  }

  createFacade(container, videoId) {
    console.log(`Creando fachada para video: ${videoId}`)

    // Crear la imagen de miniatura directamente con la URL de YouTube
    const thumbnailUrl = `https://img.youtube.com/vi/${videoId}/maxresdefault.jpg`
    const fallbackUrl = `https://img.youtube.com/vi/${videoId}/hqdefault.jpg`

    // Precargar la imagen para verificar si existe
    const img = new Image()
    img.onload = () => {
      // Si la imagen cargó correctamente y no es la imagen de error de YouTube (120x90)
      if (img.naturalWidth > 120) {
        container.style.backgroundImage = `url(${thumbnailUrl})`
        console.log(`Miniatura cargada: ${thumbnailUrl}`)
      } else {
        // Si no existe maxresdefault, usar hqdefault
        container.style.backgroundImage = `url(${fallbackUrl})`
        console.log(`Usando miniatura alternativa: ${fallbackUrl}`)
      }
    }
    img.onerror = () => {
      // Si hay error, usar hqdefault
      container.style.backgroundImage = `url(${fallbackUrl})`
      console.log(`Error al cargar miniatura, usando alternativa: ${fallbackUrl}`)
    }
    img.src = thumbnailUrl

    // Crear el botón de reproducción
    const playButton = document.createElement("button")
    playButton.className = this.options.playButtonClass
    playButton.setAttribute("aria-label", "Reproducir video")
    playButton.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
        <path d="M8 5v14l11-7z"/>
      </svg>
    `
    container.appendChild(playButton)
    console.log("Botón de reproducción agregado")
  }

  loadYouTubePlayer(container, videoId) {
    console.log(`Cargando reproductor para video: ${videoId}`)

    // Mostrar indicador de carga
    container.classList.add(this.options.loadingClass)

    // Eliminar el botón de reproducción
    const playButton = container.querySelector(`.${this.options.playButtonClass}`)
    if (playButton) {
      playButton.remove()
    }

    // Crear el iframe de YouTube
    const iframe = document.createElement("iframe")
    iframe.className = this.options.playerClass
    iframe.src = `https://www.youtube.com/embed/${videoId}?autoplay=1&rel=0`
    iframe.allow = "accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
    iframe.allowFullscreen = true
    iframe.title = "Video de Feria Sámara Market"

    // Reemplazar la fachada con el iframe
    container.innerHTML = ""
    container.appendChild(iframe)
    container.classList.remove(this.options.loadingClass)

    // Eliminar el evento de clic y la imagen de fondo
    container.style.backgroundImage = "none"
    container.style.cursor = "default"

    // Marcar como cargado
    container.dataset.loaded = "true"
    console.log("Reproductor cargado correctamente")
  }

  // Método estático para inicializar fácilmente
  static init(options) {
    return new YouTubeFacade(options)
  }
}

// Inicializar la fachada
if (typeof window !== "undefined") {
  console.log("Inicializando YouTubeFacade")
  window.youtubeFacade = YouTubeFacade.init()
}
