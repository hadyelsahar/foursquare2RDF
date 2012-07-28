/// <reference path="/jquery-1.7.2.min.js" />
/// <reference path="/jquery-1.4.1-vsdoc.js" />
var statisticsURL = "http://localhost:83/venue2rdf.svc/json/getStatistics";
var searchVenues = "http://localhost:83/venue2rdf.svc/json/getVenues";
var addToRDF_near = "http://localhost:83/Venue2rdf.svc/json/addVenuesToGraph";

function Data_loadStatistics() {

    $.ajax({
        type: "get",
        url: statisticsURL + "?callback=?",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "jsonp",
        success: function (msg) {

            eval("var x = " + msg);
            console.log(msg);
            GUI_loadStatistics(x);
        }
        , crossDomain: true
    });
}

function Data_searchVenues(brandName) {
    $(".searchresults .results").hide();
    $(".searchresults #loading").show();

    $.ajax({
        type: "get",
        url: searchVenues + "?callback=?" + "&BrandName=" + brandName,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "jsonp",
        success: function (msg) {
            console.log(msg);
            eval("var x = " + msg);
            if (x.notification.success) {
                GUI_searchVenues(x);
                $(".searchresults").slideDown(1000);
            }
        }
        , crossDomain: true
    });
}

function Data_addToRDF(brandName, near, callback) {

    $.ajax({
        type: "get",
        url: addToRDF_near + "?callback=?" + "&venue=" + brandName + "&near=" + near,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "jsonp",
        success: function (msg) {

            eval("var x = " + msg);
            console.log(msg);
            callback();
        }
        , crossDomain: true
    });
}



function GUI_loadStatistics(data) {
animateNumbers ( $("#venuesCount") ,data.venuesCount , $("#venuesCount").text() *1);
animateNumbers ( $("#categoryCount") ,data.categoriesCount , $("#categoryCount").text() *1);
animateNumbers ( $("#brandsCount") ,data.BrandsCount , $("#brandsCount").text() *1);
//    $("#venuesCount").text("venues: " + data.venuesCount);
//    $("#categoryCount").text("Categories: " + data.categoriesCount);
//    $("#brandsCount").text("brands: " + data.BrandsCount);
}

function GUI_searchVenues(data) {

    console.log(data);
    if (data.notification.success) {

        totalcheckins = 0;
        //calculating total checkins
        for (i in data.brand.venues) {
            totalcheckins += data.brand.venues[i].checkinCount * 1
        }

        //hide loading img
        $(".searchresults #loading").hide();

        //adding brand name and global statistics
        html = "<span class=\"ownercompany field\">" + "<a href=\"" + data.brand.URI + "\" >" + data.brand.name + "</a>" + "</span> ";
        html += "<span class=\"field\"> total checkins: " + totalcheckins + " </span>";
        $("#ownercompany").html(html);
        $("#venues").html("");
        html = "";
        for (i in data.brand.venues) {
            html += "<div class=\"venue\">";
            html += "<span class=\"field\">" + data.brand.venues[i].name + "</span>";
            html += "<span class=\"field\">checkins :" + data.brand.venues[i].checkinCount + "</span> ";
            html += "<span class=\"field\">users :" + data.brand.venues[i].tipsCount + "</span>";
            html += "<span class=\"field\">tips :" + data.brand.venues[i].userCount + "</span>";
            html += "<img class=\"map\" src=\"http://maps.googleapis.com/maps/api/staticmap?center=" + data.brand.venues[i].latitude + "," + data.brand.venues[i].longtitude + "&zoom=13&size=400x90&sensor=false";
            html += "&maptype=roadmap&markers=color:blue%7Clabel:L%7C" + data.brand.venues[i].latitude + "," + data.brand.venues[i].longtitude + "&key=AIzaSyCpxDCBUBLVAMI7hn1Ak7HaQrd-PCBCUcI\">";

            html += "<div class=\"clearfix\"></div></div>";
        }

        $("#venues").append(html);
        $(".searchresults .results").show();
        console.log(totalcheckins);


    }
    else {

        //couldn't load messages

    }
}

function animateNumbers(container, targetNumber, initialNumber) {

    var clr = null;
    
    inloop();

    function inloop() {
       
        if (initialNumber >= targetNumber) {
            clearTimeout(clr);
            return;
        }
        container.html(initialNumber += 1);
        clr = setTimeout(inloop, 50 *((100-(targetNumber-initialNumber))/100)); //call 'inloop()' after 50 milliseconds multiplied to the difference to make an ease effect
    }
}



$(document).ready(function () {
    $(".searchresults").hide();
    Data_loadStatistics();
    //Data_searchVenues("adidas");
    //  Data_addToRDF("puma", "cairo");

    //search funcitonality
    $("#searchButton").click(function () {

        Data_searchVenues($("#searchTextInput").val());

    });

    //add to RDF funcitonality
    $("#addToRDFButton").click(function () {

        Data_addToRDF($("#venueNameTextInput").val(), $("#nearTextInput").val(), function () {

            Data_searchVenues($("#venueNameTextInput").val());
            Data_loadStatistics();
        });
    });

});
