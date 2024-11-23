#pragma once
#include <cmath>
#include <limits>

#define ToFeet 3.28084
#define ToMeters 0.3048
#define ToRadians 0.01745329251
#define ToDegrees 57.2957795131
#define Pi 3.14159265359
#define TwoPi 6.28318530718
#define a 6378137
#define b 6356752.314245
#define f 0.0033528106647756234
#define aSquared 40680631590769
#define bSquared 40408299984659.163

class GeoCalc
{
public:
    static void GetEndingCoordinates(
        double startLatitude, double startLongitude, double startBearing, double distanceFeet,
        double* endLatitude, double* endLongitude);

    static void GetEndingCoordinates(
        double startLatitude, double startLongitude, double startBearing, double distanceFeet,
        double* endLatitude, double* endLongitude, double* endBearing);

    static void GetGreatCircleDistance(
        double startLatitude, double startLongitude, double endLatitude, double endLongitude,
        double* distanceFeet);

    static void GetGreatCircleDistance(
        double startLatitude, double startLongitude, double endLatitude, double endLongitude,
        double* distanceFeet, double* startBearing, double* reverseBearing);

private:
    GeoCalc();
    ~GeoCalc();
};
