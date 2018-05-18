
var now = new Date();
var nowDayOfWeek = now.getDay();
var nowDay = now.getDate();
var nowMonth = now.getMonth();
var nowYear = now.getYear();
nowYear += (nowYear < 2000) ? 1900 : 0;
var getCurrentDate = new Date(nowYear, nowMonth, nowDay + 1);
var getCurrentDate = formatDate(getCurrentDate)
var getTodayDate = new Date(nowYear, nowMonth, nowDay);
var getTodayDate = formatDate(getTodayDate);
var getYesterdayDate = new Date(nowYear, nowMonth, nowDay - 1);
var getYesterdayDate = formatDate(getYesterdayDate);

var getMonthStartDate = new Date(nowYear, nowMonth, 1);
var getMonthStartDate = formatDate(getMonthStartDate);

var getWeekStartDate = new Date(nowYear, nowMonth, nowDay - nowDayOfWeek);
var getWeekStartDate = formatDate(getWeekStartDate);

var getWeekEndDate = new Date(nowYear, nowMonth, nowDay + (6 - nowDayOfWeek));
var getWeekEndDate = formatDate(getWeekEndDate);

var getPreMonthStartDate = new Date(nowYear, nowMonth - 1, 1);
var getPreMonthStartDate = formatDate(getPreMonthStartDate);

var getFirMonthStartDate = new Date(nowYear, nowMonth - 1, nowDay);
var getFirMonthStartDate = formatDate(getFirMonthStartDate);

var getTreeMonthStartDate = new Date(nowYear, nowMonth - 3, nowDay);
var getTreeMonthStartDate = formatDate(getTreeMonthStartDate);

var getYearStartDate = new Date(nowYear, nowMonth - 12, nowDay);
var getYearStartDate = formatDate(getYearStartDate);

function formatDate(date) {
    var myyear = date.getFullYear();
    var mymonth = date.getMonth() + 1;
    var myweekday = date.getDate();

    if (mymonth < 10) {
        mymonth = "0" + mymonth;
    }
    if (myweekday < 10) {
        myweekday = "0" + myweekday;
    }
    return (myyear + "-" + mymonth + "-" + myweekday);
}