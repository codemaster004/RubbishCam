
function initMap() {

    const uluru = { lat: 51.9194, lng: 19.1451 };

    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 7,
        center: uluru,
    });

    google.maps.event.addListener(map, 'click', function (event) {
        placeMarker(event.latLng);
    });

    function placeMarker(location) {
        var marker = new google.maps.Marker({
            position: location,
            map: map
        });

        return location
    }

    function addMarker(position) {
        const marker = new google.maps.Marker({
            position,
            map,
            optimized: false,
        });

        document.getElementById("lon").value = position.coords.lng
        document.getElementById("lat").value = position.coords.lat
    }

}



