<template>
  <div class="table-container">
    <div class="table-overlay" v-if="this.loading"/>
    <div v-if="this.post">
      <table class="styled-table">
        <thead>
        <tr>
          <th v-for="item in this.post.header">{{ item }}</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="(row,index) in this.post.values">
          <td>{{ row[0] }}</td>
          <td v-for="(cur,index) in row.slice(1)">{{ cur }}</td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import {useCounterStore} from '@/stores/store.js'

export default {
  data() {
    return {
      post: null,
      loading: true,
      header: [],
      rows: [],
      counterStore: useCounterStore()
    };
  },
  async created() {
    await this.fetchData()
    this.counterStore.$subscribe(async (mutation, state) => {
      await this.fetchData();
    })
  },
  methods: {
    async fetchData() {
      this.loading = true;
      var response = await fetch(`api/Currencies/GetCurrencies?dateFrom=${this.counterStore.dateFrom}&dateTo=${this.counterStore.dateTo}`);
      if (response.ok) {
        this.post = await response.json();
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.table-overlay {
  position: absolute;
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.5);
}

.table-container {
  padding: 1rem;
  position: relative;
  background-color: white;
}

.styled-table {
  table-layout: auto;
  border-collapse: collapse;
  border-spacing: 0;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.3);
  overflow: hidden;
  border-radius: 1rem;
  border: 1px solid;
  background-color: lightgray;
}

.styled-table th,
.styled-table td {
  min-width: 100px;
  border: 1px solid lightgray;
  padding: 8px 12px;
  text-align: left;
}

.styled-table thead {
  background-color: #f0f0f0;
}

.styled-table td:nth-child(1) {
  background-color: #fafafa;
}

.styled-table tbody {
  background-color: white;
}

/*

.styled-table tbody tr:nth-child(even) {
  background-color: #f9f9f9;
}
*/

/*
.styled-table tbody tr:hover {
  background-color: #e0f7fa;
}*/

/*
.table-container:hover {
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
}*/

</style>
