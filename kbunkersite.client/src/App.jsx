import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { YMaps, Map, Placemark } from '@pbe/react-yandex-maps';
import { useEffect, useState } from 'react';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';
import DataTable  from './DataTable.jsx';
import "./App.css";

const App = () => {
  const [markers, setMarkers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [history, setHistory] = useState([]);

  useEffect(() => {
    const fetchMarkers = async () => {
      try {
        const response = await fetch('data/get-bunkers');
        if (!response.ok) throw new Error('Ошибка загрузки данных');
        const data = await response.json();
        setMarkers(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    const fetchHistory = async () => {
      try {
        const response = await fetch('data/history');
        if (!response.ok) throw new Error('Ошибка загрузки данных');
        const data = await response.json();
        setHistory(data.Items || []);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchMarkers();
    fetchHistory();
  }, []);

  if (loading) return <div>Загрузка карты...</div>;
  if (error) return <div>Ошибка: {error}</div>;

  return (
    <Router>
      <div className="app-container">
        <div className="map-box">
          <div className='logo'>KBunkerMap</div>
          <div className='menu'>
            <a href="/" ><button className='menu-button'>Карта</button></a>
            <a href="/history"><button className='menu-button'>История</button></a>
            <a href="/table"><button className='menu-button'>Таблица</button></a>
          </div>

          <Routes>
            <Route path="/" element={
              <YMaps>
                <Map
                  width="100vw"
                  height="700px"
                  defaultState={{ center: [55.75, 37.57], zoom: 12 }}
                >
                  {markers.map((marker, index) => (
                    <Placemark
                      key={index}
                      geometry={[marker.latitude, marker.longitude]}
                      options={{ preset: "islands#redDotIcon" }}
                      properties={{ iconCaption: marker.code }}
                    />
                  ))}
                </Map>
              </YMaps>
            } />

            <Route path="/history" element={
              <div className='history'>
                <div className='history-title'>
                  <h3 className='text-centre'>История записей</h3>
                </div>
                {history.length > 0 ? (
                  history.map((item, index) => (
                    <div className='history-box' key={index}>
                      <h2 className='null-margin'>{item.Code}</h2>
                      <h4 className='text-right w400 null-margin mt20'>
                        <b>Добавлено:</b> {format(new Date(item.SaveTime), "d MMMM yyyy 'года в' HH:mm", { locale: ru })}
                      </h4>
                    </div>
                  ))
                ) : (
                  <div className='history-box'>
                    <h4>Нет данных истории</h4>
                  </div>
                )}
              </div>
            } />

            <Route path="/table" element={<div className='table'>
              <DataTable rows={history}/>
            </div>} />
          </Routes>
        </div>

        <div className='bottom'>
          <br />
          <br />
          <h4 className='wather-mark'>
            <b>Created by</b> <a className='wather-mark' target='blank' href='https://t.me/roman_developer'>@roman_developer</a>
          </h4>
        </div>
      </div>
    </Router>
  );
};

export default App;