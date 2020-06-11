$(function () {
    Calendars();
});

function Calendars() {
    $('.datepicker').datetimepicker({
        locale: 'ru',
        format: 'DD.MM.YYYY',
        widgetPositioning: { horizontal: 'auto', vertical: 'bottom' },
        tooltips: {
            today: "Перейти к текущему дню", clear: "Очистить", close: "Закрыть", selectMonth: "Выберите месяц", prevMonth: "Предыдущий месяц",
            nextMonth: "Следующий месяц", selectYear: "Выберите год", prevYear: "Предыдущий год", nextYear: "Следующий год", selectDecade: "Выберите декаду",
            prevDecade: "Предыдущая декада", nextDecade: "Следующая декада", prevCentury: "Прошлое столетие", nextCentury: "Следующее столетие",
            pickHour: "Выберите час", incrementHour: "Увеличить час", decrementHour: "Уменьшить час", pickMinute: "Выберите минуту",
            incrementMinute: "Увеличить минуту", decrementMinute: "Уменьшить минуту", pickSecond: "Выберите секунду", incrementSecond: "Увеличить секунду",
            decrementSecond: "Уменьшить секунду", togglePeriod: "Переключить Период", selectTime: "Выберите время"
        }
    });
}