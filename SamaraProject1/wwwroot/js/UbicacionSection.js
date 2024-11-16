﻿const UbicacionSection = () => {
    return React.createElement("section", {
        className: "py-12 bg-gradient-to-b from-white to-gray-50"
    }, React.createElement("div", {
        className: "max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"
    }, [
        React.createElement("div", {
            key: "header",
            className: "text-center mb-8"
        }, [
            React.createElement("h2", {
                className: "text-3xl font-bold text-gray-900"
            }, "Nuestra Ubicación"),
            React.createElement("p", {
                className: "mt-4 text-xl text-gray-600"
            }, "Visítanos en Playa Sámara")
        ]),
        React.createElement("div", {
            key: "content",
            className: "bg-white rounded-lg shadow-lg overflow-hidden max-w-5xl mx-auto"
        }, React.createElement("div", {
            className: "md:flex"
        }, [
            React.createElement("div", {
                key: "map",
                className: "md:w-1/2"
            }, React.createElement("div", {
                className: "h-72 md:h-96 relative"
            }, React.createElement("iframe", {
                src: "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d15721.851553205592!2d-85.54024365!3d9.88151675!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x8f9fb4f68f56d72b%3A0x8d7fbf34a5c0d0c9!2sPlaya%20Samara%2C%20Provincia%20de%20Guanacaste%2C%20Costa%20Rica!5e0!3m2!1sen!2sus!4v1654321234567!5m2!1sen!2sus",
                width: "100%",
                height: "100%",
                style: { border: 0 },
                allowFullScreen: true,
                loading: "lazy",
                referrerPolicy: "no-referrer-when-downgrade",
                className: "absolute inset-0",
                title: "Ubicación de Feria Sámara Market"
            }))),
            React.createElement("div", {
                key: "info",
                className: "p-6 md:w-1/2"
            }, [
                React.createElement("h3", {
                    className: "text-xl font-semibold text-gray-900 mb-4"
                }, "Feria Sámara Market"),
                React.createElement("div", {
                    className: "space-y-4"
                }, [
                    React.createElement("div", {
                        key: "address",
                        className: "flex items-start"
                    }, [
                        React.createElement("svg", {
                            className: "h-6 w-6 text-blue-600 mt-1",
                            fill: "none",
                            viewBox: "0 0 24 24",
                            stroke: "currentColor"
                        }, React.createElement("path", {
                            strokeLinecap: "round",
                            strokeLinejoin: "round",
                            strokeWidth: 2,
                            d: "M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
                        })),
                        React.createElement("p", {
                            className: "ml-3 text-gray-700"
                        }, "Playa Sámara, Guanacaste, Costa Rica")
                    ]),
                    React.createElement("div", {
                        key: "hours",
                        className: "flex items-start"
                    }, [
                        React.createElement("svg", {
                            className: "h-6 w-6 text-blue-600 mt-1",
                            fill: "none",
                            viewBox: "0 0 24 24",
                            stroke: "currentColor"
                        }, React.createElement("path", {
                            strokeLinecap: "round",
                            strokeLinejoin: "round",
                            strokeWidth: 2,
                            d: "M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                        })),
                        React.createElement("p", {
                            className: "ml-3 text-gray-700"
                        }, "Abierto: Lun - Dom, 9:00 AM - 6:00 PM")
                    ]),
                    React.createElement("div", {
                        key: "phone",
                        className: "flex items-start"
                    }, [
                        React.createElement("svg", {
                            className: "h-6 w-6 text-blue-600 mt-1",
                            fill: "none",
                            viewBox: "0 0 24 24",
                            stroke: "currentColor"
                        }, React.createElement("path", {
                            strokeLinecap: "round",
                            strokeLinejoin: "round",
                            strokeWidth: 2,
                            d: "M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"
                        })),
                        React.createElement("p", {
                            className: "ml-3 text-gray-700"
                        }, [
                            "Teléfono: ",
                            React.createElement("a", {
                                key: "phone-link",
                                href: "tel:+50626560000",
                                className: "text-blue-600 hover:text-blue-800"
                            }, "+506 2656 0000")
                        ])
                    ])
                ]),
                React.createElement("div", {
                    className: "mt-6"
                }, React.createElement("a", {
                    href: "https://g.co/kgs/Szpmjgh",
                    target: "_blank",
                    rel: "noopener noreferrer",
                    className: "inline-flex items-center px-4 py-2 border border-transparent text-base font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                }, "Ver en Google Maps"))
            ])
        ]))
    ]));
};

// Verificar que React esté disponible
if (typeof React === 'undefined') {
    console.error('React no está definido. Asegúrate de que React se cargue antes que este script.');
}

// Verificar que ReactDOM esté disponible
if (typeof ReactDOM === 'undefined') {
    console.error('ReactDOM no está definido. Asegúrate de que ReactDOM se cargue antes que este script.');
}

// Verificar que el elemento contenedor exista
const container = document.getElementById('ubicacion-section');
if (container) {
    ReactDOM.render(React.createElement(UbicacionSection), container);
} else {
    console.error('No se encontró el elemento con id "ubicacion-section"');
}