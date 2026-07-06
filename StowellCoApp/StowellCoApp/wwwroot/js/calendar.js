window.initializeCalendar = () => {
    var calendarEl = document.getElementById('calendar');
    if (!calendarEl) return;

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        events: [
            { title: 'Event 1', start: '2025-10-25' },
            { title: 'Event 2', start: '2025-10-26', end: '2025-10-28' }
        ]
    });

    calendar.render();
};
