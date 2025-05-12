import React from 'react';
import { DataGrid } from '@mui/x-data-grid';
import { Box, Button } from '@mui/material';

const DataTable = ({ rows }) => {  // Принимаем rows как пропс
  const columns = [
    { field: 'Id', headerName: 'ID', width: 70 },
    { field: 'Code', headerName: 'Название', width: 150 },
    { field: 'Latitude', headerName: 'Широта', width: 200 },
    { field: 'Longitude',headerName: 'Долгота', width: 200,},
    { field: 'CodeDot',headerName: 'Код', width: 200,},
    { field: 'NameDot',headerName: 'Адресс', width: 300,},
    { field: 'SaveTime',headerName: 'Время обновления', width: 300,},
  ];

  async function handleExport() {
    try {
      const response = await fetch('data/csv');
      if (!response.ok) throw new Error('Ошибка загрузки');
      
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      
      const a = document.createElement('a');
      a.href = url;
      a.download = 'export.csv';
      document.body.appendChild(a);
      a.click();
      
      window.URL.revokeObjectURL(url);
      a.remove();
  } catch (error) {
      console.error('Ошибка:', error);
      alert('Не удалось скачать файл');
  }
  };

  return (
    <Box sx={{ height: 700, width: '100%' }}>
      <Button variant="contained" onClick={() => handleExport()}>
        Экспорт в Excel
      </Button>
      <DataGrid
        getRowId={(row) => row.Id} // Используем переданные данные
        rows={rows}  // Используем переданные данные
        columns={columns}
        pageSize={5}
        rowsPerPageOptions={[5, 10]}
        disableRowSelectionOnClick
      />
    </Box>
  );
};

export default DataTable;