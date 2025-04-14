import { useEffect, useState } from "react";
import { WeatherForecast } from "./types";

function App() {
  const [weatherForecasts, setWeatherForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    fetch("/api/weatherforecast")
      .then(response => {
        if (!response.ok) {
          throw new Error(`Network error - Status: ${response.status}`);
        }

        return response.json();
      })
      .then((data: WeatherForecast[]) => {
        setWeatherForecasts(data);
        setLoading(false);
      })
      .catch(error => {
        console.error("Error fetching weather forecasts:", error);
        setError(error.message);
        setLoading(false);
      });
  }, []);

  if (loading) {
    return (
      <>
        <p className="text-center">Loading...</p>
      </>
    );
  }

  if (error) {
    return (
      <>
        <p className="text-center">Error: {error}</p>
      </>
    );
  }

  return (
    <>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temperature (°C)</th>
            <th>Temperature (°F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {weatherForecasts.map((forecast, index) => (
            <tr key={index}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

export default App;
