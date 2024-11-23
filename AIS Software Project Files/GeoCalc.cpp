#include "GeoCalc.h"

GeoCalc::GeoCalc()
{
}


GeoCalc::~GeoCalc()
{
}

void GeoCalc::GetEndingCoordinates(
    double startLatitude, double startLongitude, double startBearing, double distanceFeet, 
    double* endLatitude, double* endLongitude)
{
    double trash;
    GeoCalc::GetEndingCoordinates(startLatitude, startLongitude, startBearing, distanceFeet, 
        endLatitude, endLongitude, &trash);
}

void GeoCalc::GetEndingCoordinates(
    double startLatitude, double startLongitude, double startBearing, double distanceFeet, 
    double* endLatitude, double* endLongitude, double* endBearing)
{
    double phi1 = startLatitude * ToRadians;
    double alpha1 = startBearing * ToRadians;
    double cosAlpha1 = cos(alpha1);
    double sinAlpha1 = sin(alpha1);
    double s = distanceFeet * ToMeters;
    double tanU1 = (1.0 - f) * tan(phi1);
    double cosU1 = 1.0 / sqrt(1.0 + tanU1 * tanU1);
    double sinU1 = tanU1 * cosU1;
    double sigma1 = atan2(tanU1, cosAlpha1);
    double sinAlpha = cosU1 * sinAlpha1;
    double sin2Alpha = sinAlpha * sinAlpha;
    double cos2Alpha = 1 - sin2Alpha;
    double uSquared = cos2Alpha * (aSquared - bSquared) / bSquared;
    double A = 1 + (uSquared / 16384) * (4096 + uSquared * (-768 + uSquared * (320 - 175 * uSquared)));
    double B = (uSquared / 1024) * (256 + uSquared * (-128 + uSquared * (74 - 47 * uSquared)));
    double deltaSigma;
    double sOverbA = s / (b * A);
    double sigma = sOverbA;
    double sinSigma;
    double prevSigma = sOverbA;
    double sigmaM2;
    double cosSigmaM2;
    double cos2SigmaM2;

    for (; ; )
    {
        sigmaM2 = 2.0 * sigma1 + sigma;
        cosSigmaM2 = cos(sigmaM2);
        cos2SigmaM2 = cosSigmaM2 * cosSigmaM2;
        sinSigma = sin(sigma);
        double cosSignma = cos(sigma);

        deltaSigma = B * sinSigma * (cosSigmaM2 + (B / 4.0) * (cosSignma * (-1 + 2 * cos2SigmaM2)
            - (B / 6.0) * cosSigmaM2 * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM2)));

        sigma = sOverbA + deltaSigma;

        if (fabs(sigma - prevSigma) < 0.0000000000001) break;

        prevSigma = sigma;
    }

    sigmaM2 = 2.0 * sigma1 + sigma;
    cosSigmaM2 = cos(sigmaM2);
    cos2SigmaM2 = cosSigmaM2 * cosSigmaM2;

    double cosSigma = cos(sigma);
    sinSigma = sin(sigma);

    double phi2 = atan2(sinU1 * cosSigma + cosU1 * sinSigma * cosAlpha1,
        (1.0 - f) * sqrt(sin2Alpha + pow(sinU1 * sinSigma - cosU1 * cosSigma * cosAlpha1, 2.0)));
    double lambda = atan2(sinSigma * sinAlpha1, cosU1 * cosSigma - sinU1 * sinSigma * cosAlpha1);
    double C = (f / 16) * cos2Alpha * (4 + f * (4 - 3 * cos2Alpha));
    double L = lambda - (1 - C) * f * sinAlpha * (sigma + C * sinSigma * (cosSigmaM2 + C * cosSigma * (-1 + 2 * cos2SigmaM2)));
    double alpha2 = atan2(sinAlpha, -sinU1 * sinSigma + cosU1 * cosSigma * cosAlpha1);

    *endLatitude = phi2 * ToDegrees;
    *endLongitude = startLongitude + (L * ToDegrees);
    *endBearing = alpha2 * ToDegrees;
}

void GeoCalc::GetGreatCircleDistance(
    double startLatitude, double startLongitude, double endLatitude, double endLongitude, 
    double* distanceFeet)
{
    double trash1, trash2;
    GetGreatCircleDistance(startLatitude, startLongitude, endLatitude, endLongitude,
        distanceFeet, &trash1, &trash2);
}

void GeoCalc::GetGreatCircleDistance(
    double startLatitude, double startLongitude, double endLatitude, double endLongitude, 
    double* distanceFeet, double* startBearing, double* reverseBearing)
{
    double phi1 = startLatitude * ToRadians;
    double lambda1 = startLongitude * ToRadians;
    double phi2 = endLatitude * ToRadians;
    double lambda2 = endLongitude * ToRadians;
    double a2b2b2 = (aSquared - bSquared) / bSquared;
    double omega = lambda2 - lambda1;
    double tanphi1 = tan(phi1);
    double tanU1 = (1.0 - f) * tanphi1;
    double U1 = atan(tanU1);
    double sinU1 = sin(U1);
    double cosU1 = cos(U1);
    double tanphi2 = tan(phi2);
    double tanU2 = (1.0 - f) * tanphi2;
    double U2 = atan(tanU2);
    double sinU2 = sin(U2);
    double cosU2 = cos(U2);
    double sinU1sinU2 = sinU1 * sinU2;
    double cosU1sinU2 = cosU1 * sinU2;
    double sinU1cosU2 = sinU1 * cosU2;
    double cosU1cosU2 = cosU1 * cosU2;
    double lambda = omega;
    double A = 0.0;
    double B = 0.0;
    double sigma = 0.0;
    double deltasigma = 0.0;
    double lambda0;
    bool converged = false;

    for (int i = 0; i < 20; i++)
    {
        lambda0 = lambda;
        double sinlambda = sin(lambda);
        double coslambda = cos(lambda);
        double sin2sigma = (cosU2 * sinlambda * cosU2 * sinlambda) + pow(cosU1sinU2 - sinU1cosU2 * coslambda, 2.0);
        double sinsigma = sqrt(sin2sigma);
        double cossigma = sinU1sinU2 + (cosU1cosU2 * coslambda);
        sigma = atan2(sinsigma, cossigma);
        double sinalpha = (sin2sigma == 0) ? 0.0 : cosU1cosU2 * sinlambda / sinsigma;
        double alpha = asin(sinalpha);
        double cosalpha = cos(alpha);
        double cos2alpha = cosalpha * cosalpha;
        double cos2sigmam = cos2alpha == 0.0 ? 0.0 : cossigma - 2 * sinU1sinU2 / cos2alpha;
        double u2 = cos2alpha * a2b2b2;
        double cos2sigmam2 = cos2sigmam * cos2sigmam;
        A = 1.0 + u2 / 16384 * (4096 + u2 * (-768 + u2 * (320 - 175 * u2)));
        B = u2 / 1024 * (256 + u2 * (-128 + u2 * (74 - 47 * u2)));
        deltasigma = B * sinsigma * (cos2sigmam + B / 4 * (cossigma * (-1 + 2 * cos2sigmam2) - B / 6 * cos2sigmam * (-3 + 4 * sin2sigma) * (-3 + 4 * cos2sigmam2)));
        double C = f / 16 * cos2alpha * (4 + f * (4 - 3 * cos2alpha));
        lambda = omega + (1 - C) * f * sinalpha * (sigma + C * sinsigma * (cos2sigmam + C * cossigma * (-1 + 2 * cos2sigmam2)));
        double change = fabs((lambda - lambda0) / lambda);

        if ((i > 1) && (change < 0.0000000000001))
        {
            converged = true;
            break;
        }
    }

    *distanceFeet = b * A * (sigma - deltasigma) * ToFeet;

    if (!converged)
    {
        if (phi1 > phi2)
        {
            *startBearing = 180;
            *reverseBearing = 0;
        }
        else if (phi1 < phi2)
        {
            *startBearing = 0;
            *reverseBearing = 180;
        }
        else
        {
            *startBearing = std::numeric_limits<double>::quiet_NaN();
            *reverseBearing = std::numeric_limits<double>::quiet_NaN();
        }
    }
    else
    {
        *startBearing = atan2(cosU2 * sin(lambda), (cosU1sinU2 - sinU1cosU2 * cos(lambda)));
        *reverseBearing = atan2(cosU1 * sin(lambda), (-sinU1cosU2 + cosU1sinU2 * cos(lambda))) + Pi;

        *startBearing = fmod(fmod(*startBearing * ToDegrees, 360.0) + 360.0, 360.0);
        *reverseBearing = fmod(fmod(*reverseBearing * ToDegrees, 360.0) + 360.0, 360.0);
    }
}
