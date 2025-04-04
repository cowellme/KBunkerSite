import { YMaps, Map, Placemark } from '@pbe/react-yandex-maps';
import { useEffect, useState } from 'react'; // Добавляем хуки
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';
import "./App.css";

const App = () => {
  const [markers, setMarkers] = useState([]); // Состояние для хранения меток
  const [loading, setLoading] = useState(true); // Состояние загрузки
  const [error, setError] = useState(null); // Состояние ошибки
  const [isMap, setIsMap] = useState(true); // Состояние отображения карты
  const [history, setHistory] = useState([]); // Состояние для истории посещений [lat, lng
  // Загрузка данных с API
  useEffect(() => {
    const fetchMarkers = async () => {
      try {
        const response = await fetch('data');
        if (!response.ok) {
          throw new Error('Ошибка загрузки данных');
        }
        const data = await response.json();
        console.log(data);
        setMarkers(data); // Обновляем состояние с полученными метками
      } catch (err) {
        setError(err.message); // Сохраняем ошибку
      } finally {
        setLoading(false); // Завершаем загрузку
      }
    };

    const fetchHistory = async () => {
      try {
        const response = await fetch('data/history');
        if (!response.ok) {
          throw new Error('Ошибка загрузки данных');
        }
        const data = await response.json();
        console.log(data.Items);
        setHistory(data); // Обновляем состояние с полученными метками
      } catch (err) {
        setError(err.message); // Сохраняем ошибку
      } finally {
        setLoading(false); // Завершаем загрузку
      }
    };

    fetchMarkers();
    fetchHistory();
  }, []); // Пустой массив зависимостей = запуск только при монтировании

  if (loading) return <div>Загрузка карты...</div>;
  if (error) return <div>Ошибка: {error}</div>;

  return (
    <>
      {isMap ? (<>
        <YMaps>
          <div className="map-box">
            <div className='logo'>
              KBunkerMap
            </div>
            <div className='menu'>
              <button onClick={() => setIsMap(true)} className='menu-button'>Карта</button>
              <button onClick={() => setIsMap(false)} className='menu-button'>История</button>
            </div>
            <Map
              width="100vw"
              height="700px"
              defaultState={{ center: [55.75, 37.57], zoom: 12 }}
            >
              {markers.map((marker, index) => (
                <Placemark
                  key={index} // Важно добавить key для React
                  geometry={[marker.latitude, marker.longitude]}
                  options={{ preset: "islands#redDotIcon" }}
                  properties={{ iconCaption: marker.code }}
                />
              ))}
            </Map>
          </div>
        </YMaps>
      </>) :
        (<>
          {history ? (
            <>
              <div className="map-box">
                <div className='logo'>
                  KBunkerMap
                </div>
                <div className='menu'>
                  <button onClick={() => setIsMap(true)} className='menu-button'>Карта</button>
                  <button onClick={() => setIsMap(false)} className='menu-button'>История</button>
                </div>
                <div className='history'>

                  <div className='history-title'>
                    <h3 className='text-centre'>История записей</h3>
                  </div>
                  {history.Items.map((item, index) => (
                    <div className='history-box' key={index}>
                      <h2 className='null-margin'>{item.Code}</h2>
                      <h4 className='text-right w400 null-margin mt20'><b>Добавленно:</b> {format(new Date(item.SaveTime), "d MMMM yyyy 'года в' HH:mm", { locale: ru })}</h4>
                    </div>
                  ))}
                </div>
              </div>
            </>) : (
            <>
              <div className="map-box">
                <div className='logo'>
                  KBunkerMap
                </div>
                <div className='menu'>
                  <button onClick={() => setIsMap(true)} className='menu-button'>Карта</button>
                  <button onClick={() => setIsMap(false)} className='menu-button'>История</button>
                </div>
                <div className='history'>
                  <h2>Последнии записи:</h2>
                </div>
              </div>
            </>)}
        </>)
      }




      <div className='botton'>
        <br />
        <br />
        <br />
        <h4 className='wather-mark'>Created by <a className='wather-mark' target='blank' href='https://t.me/roman_developer'>@roman_developer</a></h4>
      </div>
    </>
  );
};

export default App;