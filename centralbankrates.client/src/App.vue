<script setup>
import CurrenciesTable from './components/CurrenciesTable.vue'
import HelloWorld from "@/components/HelloWorld.vue";
import Filter from "@/components/Filter.vue";
</script>

<template>
<!--  <input v-model="message" placeholder="edit me" />-->

  <Filter/>
  <CurrenciesTable/>
<!--  <HelloWorld/>-->

</template>

<style>
</style>
