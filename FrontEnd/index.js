// Initialize and add the map
function initMap() {
    // The location of Uluru
    const uluru = { lat: 51.9194, lng: 19.1451 };
    // The map, centered at Uluru
    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 6,
        center: uluru,
    });

    const tourStops = [
        { lat: 34.8791806, lng: -111.8265049 },
        { lat: 34.8559195, lng: -111.7988186 },
        { lat: 34.832149, lng: -111.7695277 },
        { lat: 34.823736, lng: -111.8001857 },
        { lat: 34.800326, lng: -111.7665047 },
    ];

    tourStops.forEach(position => {

        addMarker(position)

    });

    google.maps.event.addListener(map, 'click', function (event) {
        placeMarker(event.latLng);
    });

    // The marker, positioned at Uluru

}

function addMarker(position) {
    const marker = new google.maps.Marker({
        position,
        map,
        optimized: false,
    });
}

function placeMarker(location) {
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });

    return location
}