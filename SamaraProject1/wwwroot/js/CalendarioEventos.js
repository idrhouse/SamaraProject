// Componente de Calendario de Eventos
const CalendarioEventos = () => {
    const [eventos, setEventos] = React.useState([]);
    const [eventoSeleccionado, setEventoSeleccionado] = React.useState(null);
    const [mes, setMes] = React.useState(new Date());

    React.useEffect(() => {
        fetch('/Evento/ObtenerEventos')
            .then(response => response.json())
            .then(data => setEventos(data))
            .catch(error => console.error('Error al cargar eventos:', error));
    }, []);

    const obtenerDiasDelMes = (fecha) => {
        const inicio = new Date(fecha.getFullYear(), fecha.getMonth(), 1);
        const fin = new Date(fecha.getFullYear(), fecha.getMonth() + 1, 0);
        const dias = [];

        // Añadir días del mes anterior para completar la primera semana
        const primerDia = inicio.getDay();
        for (let i = primerDia - 1; i >= 0; i--) {
            const dia = new Date(inicio);
            dia.setDate(dia.getDate() - i);
            dias.push({ fecha: dia, esDelMes: false });
        }

        // Añadir días del mes actual
        for (let dia = 1; dia <= fin.getDate(); dia++) {
            dias.push({
                fecha: new Date(fecha.getFullYear(), fecha.getMonth(), dia),
                esDelMes: true
            });
        }

        return dias;
    };

    const obtenerEventosPorDia = (fecha) => {
        return eventos.filter(evento => {
            const fechaEvento = new Date(evento.fecha);
            return fechaEvento.getDate() === fecha.getDate() &&
                fechaEvento.getMonth() === fecha.getMonth() &&
                fechaEvento.getFullYear() === fecha.getFullYear();
        });
    };

    const mesAnterior = () => {
        setMes(prev => new Date(prev.getFullYear(), prev.getMonth() - 1));
    };

    const mesSiguiente = () => {
        setMes(prev => new Date(prev.getFullYear(), prev.getMonth() + 1));
    };

    const formatearFecha = (fecha) => {
        return new Intl.DateTimeFormat('es-ES', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        }).format(new Date(fecha));
    };

    const diasSemana = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
    const nombreMes = new Intl.DateTimeFormat('es-ES', { month: 'long' }).format(mes);
    const año = mes.getFullYear();
    const dias = obtenerDiasDelMes(mes);

    return React.createElement('div', { className: 'bg-white rounded-lg shadow-xl p-6' },
        // Cabecera del calendario
        React.createElement('div', { className: 'flex justify-between items-center mb-6' },
            React.createElement('h2', { className: 'text-2xl font-bold text-gray-800' },
                `${nombreMes} ${año}`
            ),
            React.createElement('div', { className: 'flex gap-2' },
                React.createElement('button', {
                    className: 'p-2 rounded-lg bg-blue-100 text-blue-600 hover:bg-blue-200',
                    onClick: mesAnterior
                }, '←'),
                React.createElement('button', {
                    className: 'p-2 rounded-lg bg-blue-100 text-blue-600 hover:bg-blue-200',
                    onClick: mesSiguiente
                }, '→')
            )
        ),
        // Días de la semana
        React.createElement('div', { className: 'grid grid-cols-7 gap-2 mb-2' },
            diasSemana.map(dia =>
                React.createElement('div', {
                    key: dia,
                    className: 'text-center font-medium text-gray-600'
                }, dia)
            )
        ),
        // Días del mes
        React.createElement('div', { className: 'grid grid-cols-7 gap-2' },
            dias.map((dia, index) => {
                const eventosDelDia = obtenerEventosPorDia(dia.fecha);
                return React.createElement('div', {
                    key: index,
                    className: `p-2 border rounded-lg ${dia.esDelMes ? 'bg-white' : 'bg-gray-50'} 
                     ${eventosDelDia.length > 0 ? 'border-blue-200' : 'border-gray-200'}`
                },
                    React.createElement('div', { className: 'text-right mb-1' },
                        dia.fecha.getDate()
                    ),
                    eventosDelDia.map(evento =>
                        React.createElement('button', {
                            key: evento.idEvento,
                            className: 'w-full text-left mb-1 p-1 text-xs bg-blue-100 rounded text-blue-700 hover:bg-blue-200',
                            onClick: () => setEventoSeleccionado(evento)
                        }, evento.nombre)
                    )
                );
            })
        ),
        // Modal de evento seleccionado
        eventoSeleccionado && React.createElement('div', {
            className: 'fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4',
            onClick: () => setEventoSeleccionado(null)
        },
            React.createElement('div', {
                className: 'bg-white rounded-lg p-6 max-w-lg w-full',
                onClick: e => e.stopPropagation()
            },
                React.createElement('h3', { className: 'text-xl font-bold mb-2' },
                    eventoSeleccionado.nombre
                ),
                eventoSeleccionado.imagenUrl && React.createElement('img', {
                    src: eventoSeleccionado.imagenUrl,
                    alt: eventoSeleccionado.nombre,
                    className: 'w-full h-48 object-cover rounded-lg mb-4'
                }),
                React.createElement('p', { className: 'text-gray-600 mb-2' },
                    formatearFecha(eventoSeleccionado.fecha)
                ),
                React.createElement('p', { className: 'text-gray-700 mb-4' },
                    eventoSeleccionado.descripcion
                ),
                React.createElement('button', {
                    className: 'w-full bg-blue-600 text-white rounded-lg py-2 hover:bg-blue-700',
                    onClick: () => setEventoSeleccionado(null)
                }, 'Cerrar')
            )
        )
    );
};

// Exportar el componente globalmente
window.CalendarioEventos = CalendarioEventos;